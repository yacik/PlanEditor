using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using PlanEditor.Entities;

namespace PlanEditor
{
    /// <summary>
    /// Interaction logic for WinBuilding.xaml
    /// </summary>
    public partial class WinBuilding : Window
    {
        public enum Mode { New, Edit}
        private readonly Dictionary<object, bool> _fields = new Dictionary<object, bool>();

        private readonly Building _mBuilding;

        public WinBuilding(Building building, Mode mode)
        {
            if (building == null) return;

            InitializeComponent();
            
            _mBuilding = building;

            FillMode();
            FillAUP();
            FillFireSignal();
            FillNotification();
            FillAntifog();
            FillInsurance();

            if (mode == Mode.Edit)
            {
                Title = "Редактирование здания";
                InitializeFields(true);
                Load();
            }
            else
            {
                Title = "Новое здание";
                BtnOk.IsEnabled = false;
                InitializeFields(false);
            }
        }

        private void Insurence_select(object sender, SelectionChangedEventArgs e)
        {
            if (Insurance.SelectedIndex == 1) 
            {
                Addition.Visibility = Visibility.Visible;
                Addition.Text = "Укажите название документа, которым застрахована гражданская ответственность перед третьими лицами";
            }
            else if (Insurance.SelectedIndex == 2)
            {
                Addition.Visibility = Visibility.Visible;
                Addition.Text = "Укажите оценку остаточной стоимости имущества третьих лиц";
            }
            else
            {
                Addition.Visibility = Visibility.Hidden;
            }
        }
        
        private void FillMode()
        {
            var lst = new List<string>
            {
                "Общеобразовательные учреждения (школа, школа-интернат, детский дом, лицей, гимназия, колледж)",
                "Учреждения начального профессионального образования (профессиональное техническое училище)",
                "Учреждения среднего профессионального образования (среднее специальное учебное заведение)",
                "Прочие внешкольные и детские учреждения",
                "Детские оздоровительные лагеря, летние детские дачи",
                "Санатории, дома отдыха, профилактории",
                "Амбулатории, поликлиники, диспансеры, медпункты, консультации",
                "Предприятия розничной торговли: универмаги, промтоварные магазины; аптеки ",
                "Предприятия рыночной торговли: крытые, оптовые рынки, торговые павильоны",
                "Предприятия общественного питания",
                "Гостиницы, мотели",
                "Спортивные сооружения",
                "Клубные и культурно-зрелищные учреждения",
                "Библиотеки",
                "Музеи",
                "Прочие здания"
            };

            FillComboBox(lst, BuildingFunctionality);
        }

        private void FillAUP()
        {
            var lst = new List<string>
            {
                "Здание оборудовано системой АУП, соответствующей требованиям нормативных документов по пожарной безопасности",
                "Оборудование здания системой АУП не требуется в соответствии с требованиями нормативных документов по пожарной безопасности",
                "Здание не оборудовано системой АУП, отвечающей требованиям нормативных документов по пожарной безопасности"
            };

            FillComboBox(lst, AUP);
        }

        private void FillFireSignal()
        {
            var lst = new List<string>
            {
                "Здание оборудовано системой пожарной сигнализации, соответствующей требованиям нормативных документов по пожарной безопасности",
                "Оборудование здания системой пожарной сигнализации не требуется в соответствии с требованиями нормативных документов по пожарной безопасности",
                "Здание не оборудовано системой пожарной сигнализации, соответствующей требованиям нормативных документов по пожарной безопасности"
            };

            FillComboBox(lst, FireSignal);
        }

        private void FillNotification()
        {
            var lst = new List<string>
            {
                "Здание оборудовано системой оповещения людей о пожаре и управления эвакуацией людей, соответствующей требованиям нормативных документов по пожарной безопасности",
                "Оборудование здания системой оповещения людей о пожаре и управления эвакуацией людей не требуется в соответствии с требованиями нормативных документов по пожарной безопасности",
                "Здание не оборудовано системой оповещения людей о пожаре или здание не оборудовано системой управления эвакуацией людей, соответствующей требованиям нормативных документов по пожарной безопасности"
            };

            FillComboBox(lst, Notification);
        }

        private void FillAntifog()
        {
            var lst = new List<string>
            {
                "Здание оборудовано системой противодымной защиты, соответствующей требованиям нормативных документов по пожарной безопасности",
                "Оборудование здания системой противодымной защиты не требуется в соответствии с требованиями нормативных документов по пожарной безопасности",
                "Здание не оборудовано системой противодымной защиты, соответствующей требованиям нормативных документов по пожарной безопасности"
            };

            FillComboBox(lst, Antifog);
        }

        private void FillInsurance()
        {
            var lst = new List<string>
            {
                "Оценка возможного ущерба имуществу третьих лиц от пожара не производилась",
                "Имущество третьего лица застраховано",
                "Оценка возможного ущерба имуществу третьих лиц от пожара",
                "Возможность ущерба имуществу третьих лиц от пожара отсутствует"
            };

            FillComboBox(lst, Insurance);

            Insurance.SelectionChanged += Insurence_select;
            Addition.Visibility = System.Windows.Visibility.Hidden;
        }
        
        private void FillComboBox(IEnumerable<string> lst, ComboBox cb)
        {
            foreach (var item in lst.Select(v => new ComboBoxItem {Content = v}))
            {
                cb.Items.Add(item);
            }
        }

        private void Click_OK(object sender, RoutedEventArgs e)
        {
            DialogResult = true;

            _mBuilding.Name = BuildingName.Text;
            _mBuilding.Address = BuildingAddress.Text;
            _mBuilding.Functionality = BuildingFunctionality.SelectedIndex;
            _mBuilding.People = int.Parse(NumPeople.Text);
            _mBuilding.MaxPeople = int.Parse(MaxPeople.Text);
            _mBuilding.FireSafetySys = AUP.SelectedIndex;
            _mBuilding.FireSignal = FireSignal.SelectedIndex;
            _mBuilding.Notification = Notification.SelectedIndex;
            _mBuilding.AntiFog = Antifog.SelectedIndex;
            _mBuilding.Insurance = Insurance.SelectedIndex;
            _mBuilding.Stages = int.Parse(Stages.Text);
            _mBuilding.Lx = double.Parse(Width.Text);
            _mBuilding.Ly = double.Parse(Length.Text);
            _mBuilding.HeightStage = double.Parse(Height.Text);

            if (_mBuilding.Insurance == 1 || _mBuilding.Insurance == 2)
            {
                _mBuilding.Addition = Addition.Text;    
            }

            InitializeLayers();
        }

        private void Load()
        {
            BuildingName.Text = _mBuilding.Name;
            BuildingAddress.Text = _mBuilding.Address;
            BuildingFunctionality.SelectedIndex = _mBuilding.Functionality;
            NumPeople.Text = _mBuilding.People.ToString();
            MaxPeople.Text = _mBuilding.MaxPeople.ToString();
            AUP.SelectedIndex = _mBuilding.FireSafetySys;
            FireSignal.SelectedIndex = _mBuilding.FireSignal;
            Notification.SelectedIndex = _mBuilding.Notification;
            Antifog.SelectedIndex = _mBuilding.AntiFog;
            Insurance.SelectedIndex = _mBuilding.Insurance;
            Stages.Text = _mBuilding.Stages.ToString();
            Width.Text = _mBuilding.Lx.ToString();
            Length.Text = _mBuilding.Ly.ToString();
            Height.Text = _mBuilding.HeightStage.ToString();

            if (_mBuilding.Insurance == 1 || _mBuilding.Insurance == 2)
            {
                Addition.Text = _mBuilding.Addition;
            }
        }

        private void InitializeLayers()
        {
            for (int i = 0; i < _mBuilding.Stages; ++i)
            {
                _mBuilding.Places.Add(new List<Place>());
                _mBuilding.Portals.Add(new List<Portal>());
            }
        }

        private void InitializeFields(bool val)
        {
            _fields.Add(NumPeople, val);
            _fields.Add(MaxPeople, val);
            _fields.Add(Stages, val);
            _fields.Add(Height, val);
            _fields.Add(Width, val);
            _fields.Add(Length, val);
        }

        private void ChengedInt(object sender, TextChangedEventArgs e)
        {
            var field = e.Source as TextBox;
            if (field != null)
            {
                int val;
                if (int.TryParse(field.Text, out val))
                {
                    _fields[field] = (val > 0);
                }
                else
                {
                    _fields[field] = false;
                }
            }

            CheckFields();
        }

        private void ChengedDouble(object sender, TextChangedEventArgs e)
        {
            var field = e.Source as TextBox;
            if (field != null)
            {
                double val;
                if (double.TryParse(field.Text, out val))
                {
                    _fields[field] = (val > 0);
                }
                else
                {
                    _fields[field] = false;
                }
            }

            CheckFields();
        }

        private void CheckFields()
        {
            var isOk = true;
            foreach (var field in _fields)
            {
                if (field.Value == false)
                {
                    isOk = false;
                    break;
                }
            }

            BtnOk.IsEnabled = isOk;
        }
    }    
}
