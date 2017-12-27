using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace PotentialCalculator.Models {
    public class Criteria : INotifyPropertyChanged {
        string _name;
        [DisplayName("Имя критерия")]
        public string Name {
            get {
                return _name;
            }
            set {
                if (_name == value) {
                    return;
                }
                _name = value;
                OnPropertyChanged(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }
        double _sourceMean;
        [DisplayName("Мат. ожидание источника")]
        public double SourceMean {
            get {
                return _sourceMean;
            }
            set {
                if (_sourceMean == value) {
                    return;
                }
                _sourceMean = value;
                OnPropertyChanged(this, new PropertyChangedEventArgs(nameof(SourceMean)));
            }
        }
        double _sourceSigma;
        [DisplayName("СКО источника")]
        public double SourceSigma {
            get {
                return _sourceSigma;
            }
            set {
                if (_sourceSigma == value) {
                    return;
                }
                _sourceSigma = value;
                OnPropertyChanged(this, new PropertyChangedEventArgs(nameof(SourceSigma)));
            }
        }
        double _disturberMean;
        [DisplayName("Мат. ожидание помехи")]
        public double DisturberMean {
            get {
                return _disturberMean;
            }
            set {
                if (_disturberMean == value) {
                    return;
                }
                _disturberMean = value;
                OnPropertyChanged(this, new PropertyChangedEventArgs(nameof(DisturberMean)));
            }
        }
        double _disturberSigma;
        [DisplayName("СКО помехи")]
        public double DisturberSigma {
            get {
                return _disturberSigma;
            }
            set {
                if (_disturberSigma == value) {
                    return;
                }
                _disturberSigma = value;
                OnPropertyChanged(this, new PropertyChangedEventArgs(nameof(DisturberSigma)));
            }
        }
        double threshold;
        [DisplayName("Порог")]
        public double Threshold {
            get { return threshold; }
            set {
                if (threshold == value)
                    return;
                threshold = value;
                OnPropertyChanged(this, new PropertyChangedEventArgs(nameof(Threshold)));
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
            PropertyChanged?.Invoke(sender, e);
        }
    }
}
