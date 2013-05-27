using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PlanEditor
{
    /// <summary>
    /// Interaction logic for Building.xaml
    /// </summary>
    public partial class Building : Window
    {
        public enum Mode { New, Edit}

        private Entities.Building m_Building;

        public Building(Entities.Building building, Mode mode)
        {
            m_Building = building;
            
            InitializeComponent();

            FillMode();
            FillAUP();
            FillFireSignal();
            FillNotification();
            FillAntifog();
            FillInsurance();

            if (mode == Mode.Edit)
            {
                Load();
            }
        }

        private void Insurence_select(object sender, SelectionChangedEventArgs e)
        {
            if (Insurance.SelectedIndex == 1) 
            {
                Addition.Visibility = System.Windows.Visibility.Visible;
                Addition.Text = "Укажите название документа, которым застрахована гражданская ответственность перед третьими лицами";
            }
            else if (Insurance.SelectedIndex == 2)
            {
                Addition.Visibility = System.Windows.Visibility.Visible;
                Addition.Text = "Укажите оценку остаточной стоимости имущества третьих лиц";
            }
            else
            {
                Addition.Visibility = System.Windows.Visibility.Hidden;
            }
        }
        
        private void FillMode()
        {
            var lst = new List<string>();
                       
            lst.Add("Общеобразовательные учреждения (школа, школа-интернат, детский дом, лицей, гимназия, колледж)"); 
            lst.Add("Учреждения начального профессионального образования (профессиональное техническое училище)");
            lst.Add("Учреждения среднего профессионального образования (среднее специальное учебное заведение)");
            lst.Add("Прочие внешкольные и детские учреждения");
            lst.Add("Детские оздоровительные лагеря, летние детские дачи");
            lst.Add("Санатории, дома отдыха, профилактории");
            lst.Add("Амбулатории, поликлиники, диспансеры, медпункты, консультации");
            lst.Add("Предприятия розничной торговли: универмаги, промтоварные магазины; аптеки ");
            lst.Add("Предприятия рыночной торговли: крытые, оптовые рынки, торговые павильоны");
            lst.Add("Предприятия общественного питания");
            lst.Add("Гостиницы, мотели");
            lst.Add("Спортивные сооружения");
            lst.Add("Клубные и культурно-зрелищные учреждения");
            lst.Add("Библиотеки");
            lst.Add("Музеи");
            lst.Add("Прочие здания");

            FillComboBox(lst, BuildingFunctionality);
        }

        private void FillAUP()
        {
            var lst = new List<string>();

            lst.Add("Здание оборудовано системой АУП, соответствующей требованиям нормативных документов по пожарной безопасности");
            lst.Add("Оборудование здания системой АУП не требуется в соответствии с требованиями нормативных документов по пожарной безопасности");
            lst.Add("Здание не оборудовано системой АУП, отвечающей требованиям нормативных документов по пожарной безопасности");

            FillComboBox(lst, AUP);
        }

        private void FillFireSignal()
        {
            var lst = new List<string>();
            
            lst.Add("Здание оборудовано системой пожарной сигнализации, соответствующей требованиям нормативных документов по пожарной безопасности");
            lst.Add("Оборудование здания системой пожарной сигнализации не требуется в соответствии с требованиями нормативных документов по пожарной безопасности");
            lst.Add("Здание не оборудовано системой пожарной сигнализации, соответствующей требованиям нормативных документов по пожарной безопасности");

            FillComboBox(lst, FireSignal);
        }

        private void FillNotification()
        {
            var lst = new List<string>();

            lst.Add("Здание оборудовано системой оповещения людей о пожаре и управления эвакуацией людей, соответствующей требованиям нормативных документов по пожарной безопасности");
            lst.Add("Оборудование здания системой оповещения людей о пожаре и управления эвакуацией людей не требуется в соответствии с требованиями нормативных документов по пожарной безопасности");
            lst.Add("Здание не оборудовано системой оповещения людей о пожаре или здание не оборудовано системой управления эвакуацией людей, соответствующей требованиям нормативных документов по пожарной безопасности");

            FillComboBox(lst, Notification);
        }

        private void FillAntifog()
        {
            var lst = new List<string>();

            lst.Add("Здание оборудовано системой противодымной защиты, соответствующей требованиям нормативных документов по пожарной безопасности");
            lst.Add("Оборудование здания системой противодымной защиты не требуется в соответствии с требованиями нормативных документов по пожарной безопасности");
            lst.Add("Здание не оборудовано системой противодымной защиты, соответствующей требованиям нормативных документов по пожарной безопасности");

            FillComboBox(lst, Antifog);
        }

        private void FillInsurance()
        {
            var lst = new List<string>();

            lst.Add("Оценка возможного ущерба имуществу третьих лиц от пожара не производилась");
            lst.Add("Имущество третьего лица застраховано"); //Название документа, которым застрахована гражданская ответственность перед третьими лицами
            lst.Add("Оценка возможного ущерба имуществу третьих лиц от пожара"); //Оценка остаточной стоимости имущества третьих лиц
            lst.Add("Возможность ущерба имуществу третьих лиц от пожара отсутствует");

            FillComboBox(lst, Insurance);

            Insurance.SelectionChanged += Insurence_select;
            Addition.Visibility = System.Windows.Visibility.Hidden;
        }
        
        private void FillComboBox(List<string> lst, ComboBox cb)
        {
            foreach (var v in lst)
            {
                var item = new ComboBoxItem();
                item.Content = v;
                cb.Items.Add(item);
            }
        }

        private void Click_OK(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;

            m_Building.Name = BuildingName.Text;
            m_Building.Address = BuildingAddress.Text;
            m_Building.Functionality = BuildingFunctionality.SelectedIndex;
            m_Building.People = int.Parse(NumPeople.Text);
            m_Building.MaxPeople = int.Parse(MaxPeople.Text);
            m_Building.FireSafetySys = AUP.SelectedIndex;
            m_Building.FireSignal = FireSignal.SelectedIndex;
            m_Building.Notification = Notification.SelectedIndex;
            m_Building.AntiFog = Antifog.SelectedIndex;
            m_Building.Insurance = Insurance.SelectedIndex;
            m_Building.Stages = int.Parse(Stages.Text);
            m_Building.Lx = double.Parse(Width.Text);
            m_Building.Ly = double.Parse(Length.Text);
            m_Building.HeightStage = double.Parse(Height.Text);

            if (m_Building.Insurance == 1 || m_Building.Insurance == 2)
            {
                m_Building.Addition = Addition.Text;    
            }

            InitializeLayers();
        }

        private void Load()
        {
            BuildingName.Text = m_Building.Name;
            BuildingAddress.Text = m_Building.Address;
            BuildingFunctionality.SelectedIndex = m_Building.Functionality;
            NumPeople.Text = m_Building.People.ToString();
            MaxPeople.Text = m_Building.MaxPeople.ToString();
            AUP.SelectedIndex = m_Building.FireSafetySys;
            FireSignal.SelectedIndex = m_Building.FireSignal;
            Notification.SelectedIndex = m_Building.Notification;
            Antifog.SelectedIndex = m_Building.AntiFog;
            Insurance.SelectedIndex = m_Building.Insurance;
            Stages.Text = m_Building.Stages.ToString();
            Width.Text = m_Building.Lx.ToString();
            Length.Text = m_Building.Ly.ToString();
            Height.Text = m_Building.HeightStage.ToString();

            if (m_Building.Insurance == 1 || m_Building.Insurance == 2)
            {
                Addition.Text = m_Building.Addition;
            }
        }

        private void InitializeLayers()
        {
            for (int i = 0; i < m_Building.Stages; ++i)
            {
                m_Building.Places.Add(new List<PlanEditor.Entities.Place>());
                m_Building.Portals.Add(new List<PlanEditor.Entities.Portal>());
            }
        }
    }    
}
