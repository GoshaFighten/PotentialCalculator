using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PotentialCalculator.Helpers;
using PotentialCalculator.Models;
using System.Collections.Generic;
using System.Linq;

namespace Tests {
    [TestClass]
    public class CCTests {
        List<Criteria> criterias = new List<Criteria>() {
            new Criteria() {
                Name = "Criteria 1",
                SourceMean = 100.0,
                SourceSigma = 10.0,
                DisturberMean = 80.0,
                DisturberSigma = 5.0,
                Threshold = 90.0
            },
            new Criteria() {
                Name = "Criteria 2",
                SourceMean = 20.0,
                SourceSigma = 5.0,
                DisturberMean = 12.0,
                DisturberSigma = 3.0,
                Threshold = 15.0
            }};

        int precision = 6;

        double currentValue1Source = 98.0;
        double currentValue2Source = 21.0;
        double currentValue1Disturber = 84.0;
        double currentValue2Disturber = 10.0;

        Dictionary<Criteria, Tuple<double, double>> criteriasWithValues = new Dictionary<Criteria, Tuple<double, double>>();

        public CCTests() {
            this.criteriasWithValues.Add(this.criterias[0], new Tuple<double, double>(this.currentValue1Source, this.currentValue1Disturber));
            this.criteriasWithValues.Add(this.criterias[1], new Tuple<double, double>(this.currentValue2Source, this.currentValue2Disturber));
        }

        [TestMethod]
        public void TestCalcNormalization() {
            PrivateType pt = new PrivateType(typeof(MyMath));
            var criterias = this.criterias;
            var normalization = (double)pt.InvokeStatic("CalcNormalization", criterias);
            Assert.AreEqual(0.3, Math.Round(normalization, precision));
        }

        [TestMethod]
        public void TestCalcKi1() {
            PrivateType pt = new PrivateType(typeof(MyMath));
            var sigma = criterias[0].SourceSigma;
            var normalization = (double)pt.InvokeStatic("CalcNormalization", criterias);
            var ki = (double)pt.InvokeStatic("CalcKi", sigma, normalization);
            Assert.AreEqual(Math.Round(1.0 / 3.0, precision), Math.Round(ki, precision));
        }

        [TestMethod]
        public void TestCalcKi2() {
            PrivateType pt = new PrivateType(typeof(MyMath));
            var sigma = criterias[1].SourceSigma;
            var normalization = (double)pt.InvokeStatic("CalcNormalization", criterias);
            var ki = (double)pt.InvokeStatic("CalcKi", sigma, normalization);
            Assert.AreEqual(Math.Round(2.0 / 3.0, precision), Math.Round(ki, precision));
        }

        [TestMethod]
        public void TestCalcKs() {
            PrivateType pt = new PrivateType(typeof(MyMath));
            var criterias = this.criterias;
            var ks = (Dictionary<Criteria, double>)pt.InvokeStatic("CalcKs", criterias);
            Assert.AreEqual(Math.Round(1.0 / 3.0, precision), Math.Round(ks[criterias[0]], precision));
            Assert.AreEqual(Math.Round(2.0 / 3.0, precision), Math.Round(ks[criterias[1]], precision));
        }

        [TestMethod]
        public void TestCalcFi1Source() {
            PrivateType pt = new PrivateType(typeof(MyMath));
            var k = ((Dictionary<Criteria, double>)pt.InvokeStatic("CalcKs", criterias))[criterias[0]];
            var mean = criterias[0].SourceMean;
            var currentValue = this.criteriasWithValues[criterias[0]].Item1;
            var fi = (double)pt.InvokeStatic("CalcFi", k, mean, currentValue);
            Assert.AreEqual(0.000136, Math.Round(fi, precision));
        }

        [TestMethod]
        public void TestCalcFi2Source() {
            PrivateType pt = new PrivateType(typeof(MyMath));
            var k = ((Dictionary<Criteria, double>)pt.InvokeStatic("CalcKs", criterias))[criterias[1]];
            var mean = criterias[1].SourceMean;
            var currentValue = this.criteriasWithValues[criterias[1]].Item1;
            var fi = (double)pt.InvokeStatic("CalcFi", k, mean, currentValue);
            Assert.AreEqual(0.001587, Math.Round(fi, precision));
        }

        [TestMethod]
        public void TestCalcFi1Disturber() {
            PrivateType pt = new PrivateType(typeof(MyMath));
            var k = ((Dictionary<Criteria, double>)pt.InvokeStatic("CalcKs", criterias))[criterias[0]];
            var mean = criterias[0].SourceMean;
            var currentValue = this.criteriasWithValues[criterias[0]].Item2;
            var fi = (double)pt.InvokeStatic("CalcFi", k, mean, currentValue);
            Assert.AreEqual(0.010159, Math.Round(fi, precision));
        }

        [TestMethod]
        public void TestCalcFi2Disturber() {
            PrivateType pt = new PrivateType(typeof(MyMath));
            var k = ((Dictionary<Criteria, double>)pt.InvokeStatic("CalcKs", criterias))[criterias[1]];
            var mean = criterias[1].SourceMean;
            var currentValue = this.criteriasWithValues[criterias[1]].Item2;
            var fi = (double)pt.InvokeStatic("CalcFi", k, mean, currentValue);
            Assert.AreEqual(Math.Round(1.0 / 3.0, precision), Math.Round(fi, precision));
        }

        [TestMethod]
        public void TestCalcCC() {
            PrivateType pt = new PrivateType(typeof(MyMath));
            var k = (Tuple<double, double>)pt.InvokeStatic("CalcCC", criteriasWithValues);
            Assert.AreEqual(0.001723, Math.Round(k.Item1, precision));
            Assert.AreEqual(0.343492, Math.Round(k.Item2, precision));
        }
    }
}
