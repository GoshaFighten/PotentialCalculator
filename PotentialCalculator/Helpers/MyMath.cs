﻿using Meta.Numerics.Statistics.Distributions;
using PotentialCalculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PotentialCalculator.Helpers {
    public static class MyMath {
        static Random random = new Random();
        static int retry = 10000000;
        static int precision = 5;
        public static MyPoint[] GetGraphDataForNormalDistribution(double mean, double sigma) {
            var list = new List<MyPoint>();
            var d = new NormalDistribution(mean, sigma);
            var min = mean - (3.1 * sigma);
            var max = mean + (3.1 * sigma);
            var step = (max - min) / 1000 > 0.1 ? 0.1 : (max - min) / 1000;
            for (double i = min; i < max; i += step) {
                list.Add(new MyPoint(i, d.ProbabilityDensity(i)));
            }
            return list.ToArray();
        }

        public static Tuple<MyPoint[], MyPoint[], Dictionary<CCDecision, double>> CalcCCDensity(List<Criteria> criterias, double threshold, bool newLogic = false) {
            var ccSourceDensity = new Dictionary<double, double>();
            var ccDisturberDensity = new Dictionary<double, double>();
            var decisions = new Dictionary<CCDecision, double>();
            decisions.Add(CCDecision.Hit, 0.0);
            decisions.Add(CCDecision.FalseAlarm, 0.0);
            decisions.Add(CCDecision.Miss, 0.0);
            for (int i = 0; i < retry; i++) {
                double ccSource, ccDisturber;
                var cc = CalcCCRandom(criterias, newLogic);
                ccSource = Math.Round(cc.Item1, precision);
                ccDisturber = Math.Round(cc.Item2, precision);
                if (ccSourceDensity.ContainsKey(ccSource)) {
                    ccSourceDensity[ccSource] += 1.0 / retry;
                } else {
                    ccSourceDensity.Add(ccSource, 1.0 / retry);
                }
                if (ccDisturberDensity.ContainsKey(ccDisturber)) {
                    ccDisturberDensity[ccDisturber] += 1.0 / retry;
                } else {
                    ccDisturberDensity.Add(ccDisturber, 1.0 / retry);
                }
                var decision = MakeDecision(ccSource, ccDisturber, threshold);
                if (decisions.ContainsKey(decision)) {
                    decisions[decision] += 1.0 / retry;
                } else {
                    decisions.Add(decision, 1.0 / retry);
                }
            }
            return new Tuple<MyPoint[], MyPoint[], Dictionary<CCDecision, double>>(
                ccSourceDensity.OrderBy(i => i.Key).Select(i => new MyPoint() { X = i.Key, Y = i.Value }).ToArray(),
                ccDisturberDensity.OrderBy(i => i.Key).Select(i => new MyPoint() { X = i.Key, Y = i.Value }).ToArray(),
                decisions
            );
        }

        static Tuple<double, double> CalcCCRandom(List<Criteria> criterias, bool newLogic = false) {
            var data = new Dictionary<Criteria, Tuple<double, double>>();
            foreach (var criteria in criterias) {
                var sourceCriteriaDistribution = new NormalDistribution(criteria.SourceMean, criteria.SourceSigma);
                var disturberCriteriaDistribution = new NormalDistribution(criteria.DisturberMean, criteria.DisturberSigma);
                var currentSourceCriteriaValue = sourceCriteriaDistribution.GetRandomValue(MyMath.random);
                var currentDisturberCriteriaValue = disturberCriteriaDistribution.GetRandomValue(MyMath.random);
                data.Add(criteria, new Tuple<double, double>(currentSourceCriteriaValue, currentDisturberCriteriaValue));
            }
            return CalcCC(data, newLogic);
        }

        static Tuple<double, double> CalcCC(Dictionary<Criteria, Tuple<double, double>> criterias, bool newLogic = false) {
            double ccSource = 0.0;
            double ccDisturber = 0.0;
            var ks = CalcKs(criterias.Keys, newLogic);
            foreach (var criteria in criterias) {
                ccSource += CalcFi(ks[criteria.Key], criteria.Key.SourceMean, criteria.Value.Item1);
                ccDisturber += CalcFi(ks[criteria.Key], criteria.Key.SourceMean, criteria.Value.Item2);
            }
            return new Tuple<double, double>(ccSource, ccDisturber);
        }

        public static Dictionary<Criteria, double> CalcKs(IEnumerable<Criteria> criterias, bool newLogic = false) {
            var normalization = CalcNormalization(criterias, newLogic);
            var ks = new Dictionary<Criteria, double>();
            foreach (var criteria in criterias) {
                ks.Add(criteria, CalcKi(criteria.SourceSigma, normalization, newLogic, criteria.SourceMean));
            }
            return ks;
        }

        static double CalcNormalization(IEnumerable<Criteria> criterias, bool newLogic = false) {
            if (newLogic) {
                return criterias.Select(c => c.SourceMean / c.SourceSigma).Sum();
            }
            return criterias.Select(c => 1.0 / c.SourceSigma).Sum();
        }

        static double CalcKi(double sigma, double normalization, bool newLogic = false, double mean = 0.0) {
            if (newLogic) {
                return mean / sigma / normalization;
            }
            return 1.0 / sigma / normalization;
        }

        static double CalcFi(double k, double mean, double currentValue) {
            return Math.Abs(k * Math.Pow((currentValue - mean), 2) / currentValue / mean);
        }

        static CCDecision MakeDecision(double FSource, double FDisturber, double threshold) {
            var targetF = Math.Min(FSource, FDisturber);
            if (threshold < targetF) {
                return CCDecision.Miss;
            }
            return targetF == FSource ? CCDecision.Hit : CCDecision.FalseAlarm;
        }

        public static double CalcDetectionP(Criteria criteria) {
            var d = new NormalDistribution(criteria.SourceMean, criteria.SourceSigma);
            if (criteria.SourceMean > criteria.DisturberMean) {
                return d.RightProbability(criteria.Threshold);
            }
            return d.LeftProbability(criteria.Threshold);
        }
        public static double CalcErrorP(Criteria criteria) {
            var d = new NormalDistribution(criteria.DisturberMean, criteria.DisturberSigma);
            if (criteria.SourceMean > criteria.DisturberMean) {
                return d.RightProbability(criteria.Threshold);
            }
            return d.LeftProbability(criteria.Threshold);
        }
        public static double CalcMissP(Criteria criteria) {
            var d = new NormalDistribution(criteria.SourceMean, criteria.DisturberSigma);
            if (criteria.SourceMean > criteria.DisturberMean) {
                return d.LeftProbability(criteria.Threshold);
            }
            return d.RightProbability(criteria.Threshold);
        }
        public static double CalcRandomLeftProbability(MyPoint[] density, double threshold) {
            return density.Where(p => p.X < threshold).Select(p => p.Y).Sum();
        }
        public static double CalcRandomMean(MyPoint[] density) {
            return density.Sum(p => p.X * p.Y);
        }
        public static double GetMaxY(MyPoint[] one, MyPoint[] two) {
            var maxYOne = one.Max(p => p.Y);
            var maxYTwo = two.Max(p => p.Y);
            return Math.Max(maxYOne, maxYTwo);
        }
    }
}
