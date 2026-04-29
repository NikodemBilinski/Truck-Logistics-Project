using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;

namespace TrucksLogisticsClient.Models
{
    public class Language : INotifyPropertyChanged
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        [JsonIgnore]
        public List<Users> Users { get; set; } = new();


        [JsonIgnore]
        private Color _selectioncolor = Colors.Transparent;

        [JsonIgnore]
        public Color? SelectionColor
        {
            get 
            { 
                return _selectioncolor; 
            }
            set
            {
                if (_selectioncolor != value)
                {
                    _selectioncolor = value;
                    OnPropertyChanged(nameof(SelectionColor));
                }
            } 
        }


       public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyname = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }



    }
}
