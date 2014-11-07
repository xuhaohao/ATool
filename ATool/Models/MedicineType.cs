using System.ComponentModel;

namespace ATool.Models
{
    public class MedicineType : INotifyPropertyChanged
    {
        private string _pk;

        private string _strName;

        private float _price;

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

        public string StrName
        {
            get
            {
                return this._strName;
            }
            set
            {
                if ((object.ReferenceEquals(this._strName, value) != true))
                {
                    this._strName = value;
                    this.RaisePropertyChanged("StrName");
                }
            }
        }

        public float Price
        {
            get
            {
                return this._price;
            }
            set
            {
                if ((object.ReferenceEquals(this._price, value) != true))
                {
                    this._price = value;
                    this.RaisePropertyChanged("Price");
                }
            }
        }

        public MedicineType()
        {
            //for Serialize
        }

        public MedicineType(string strName, float price)
        {
            _strName = strName;
            _price = price;
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
