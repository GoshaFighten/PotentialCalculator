using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;

namespace PotentialCalculator.Models {
    public class Project: INotifyPropertyChanged {
        private Project() {
            Criterias = new BindingList<Criteria>();
        }
        private static Project fProjectInstance;

        protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
            PropertyChanged?.Invoke(sender, e);
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public static Project ProjectInstance {
            get {
                if (fProjectInstance == null) {
                    fProjectInstance = new Project();
                }
                return fProjectInstance;
            }
        }
        double cCThreshold;
        [DisplayName("КК Порог")]
        public double CCThreshold {
            get {
                return cCThreshold;
            }
            set {
                if (cCThreshold == value) {
                    return;
                }

                cCThreshold = value;
                OnPropertyChanged(this, new PropertyChangedEventArgs(nameof(CCThreshold)));
            }
        }
        public BindingList<Criteria> Criterias { get; private set; }
    }
}
