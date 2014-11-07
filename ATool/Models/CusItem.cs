using System.ComponentModel;

namespace ATool.Models
{
    class CusItem : INotifyPropertyChanged
    {
        private string _pk;

        private string _doctorName;

        private MedicineType _medicine;

        private int _number;

        private float _sumPrice;

        public string Pk
        {
            get
            {
                return this._pk;
            }
            set
            {
                if ((object.ReferenceEquals(this._pk, value) != true))
                {
                    this._pk = value;
                    this.RaisePropertyChanged("Pk");
                }
            }
        }

        public string DoctorName
        {
            get
            {
                return this._doctorName;
            }
            set
            {
                if ((object.ReferenceEquals(this._doctorName, value) != true))
                {
                    this._doctorName = value;
                    this.RaisePropertyChanged("Pk");
                }
            }
        }

        public MedicineType Medicine
        {
            get
            {
                return this._medicine;
            }
            set
            {
                if ((object.ReferenceEquals(this._medicine, value) != true))
                {
                    this._medicine = value;
                    this.RaisePropertyChanged("Medicine");
                }
            }
        }

        public int Number
        {
            get
            {
                return this._number;
            }
            set
            {
                if ((object.ReferenceEquals(this._number, value) != true))
                {
                    this._number = value;
                    this.RaisePropertyChanged("Number");
                }
            }
        }

        public float SumPrice
        {
            get
            {
                return this._sumPrice;
            }
            set
            {
                if ((object.ReferenceEquals(this._sumPrice, value) != true))
                {
                    this._sumPrice = value;
                    this.RaisePropertyChanged("SumPrice");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            var propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
