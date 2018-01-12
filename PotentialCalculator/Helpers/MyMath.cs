using Meta.Numerics.Statistics.Distributions;
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
            for (double i = min; i < max; i += 0.1) {
                list.Add(new MyPoint(i, d.ProbabilityDensity(i)));
            }
            return list.ToArray();
        }

        public static Tuple<MyPoint[], MyPoint[]> CalcCC(List<Criteria> criterias) {
            var normalization = criterias.Select(c => 1.0 / c.SourceSigma).Sum();
            var ccSourceDensity = new Dictionary<double, double>();
            var ccDisturberDensity = new Dictionary<double, double>();
            for (int i = 0; i < retry; i++) {
                double ccSource = 0.0;
                double ccDisturber = 0.0;
                foreach (var criteria in criterias) {
                    var sourceCriteriaDistribution = new NormalDistribution(criteria.SourceMean, criteria.SourceSigma);
                    var disturberCriteriaDistribution = new NormalDistribution(criteria.DisturberMean, criteria.DisturberSigma);
                    var currentSourceCriteriaValue = sourceCriteriaDistribution.GetRandomValue(MyMath.random);
                    var currentDisturberCriteriaValue = disturberCriteriaDistribution.GetRandomValue(MyMath.random);
                    ccSource += CalcF(criteria.SourceMean, criteria.SourceSigma, normalization, currentSourceCriteriaValue);
                    ccDisturber += CalcF(criteria.SourceMean, criteria.SourceSigma, normalization, currentDisturberCriteriaValue);
                }
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
            }
            return new Tuple<MyPoint[], MyPoint[]>(
                ccSourceDensity.OrderBy(i => i.Key).Select(i => new MyPoint() { X = i.Key, Y = i.Value }).ToArray(),
                ccDisturberDensity.OrderBy(i => i.Key).Select(i => new MyPoint() { X = i.Key, Y = i.Value }).ToArray()
            );
        }
        static double CalcF(double mean, double sigma, double normalization, double currentValue) {
            return Math.Abs(Math.Round(1.0 / sigma / normalization * Math.Pow((currentValue - mean), 2) / currentValue / mean, precision));
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
            return maxYOne > maxYTwo ? maxYOne : maxYTwo;
        }
    }
}
