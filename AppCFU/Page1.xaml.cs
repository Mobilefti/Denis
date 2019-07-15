using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppCFU
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page1 : ContentPage
    {
        private bool Switch_color = true;
        private List<Para> schedules;
        private List<Schedule> schedule;
        string week_color = "green";
        public Page1()
        {
            InitializeComponent();
            OpenServer();
            schedule = new List<Schedule>();
            schedules = new List<Para>();
        }
        private void OpenServer()
        {
            try
            {
                //string getUrl = "http://192.168.43.157:8080/PTI/login";//IP server
                string getUrl = "http://192.168.0.101:8080/PTI/login";//IP server
                string postData = String.Format("email={0}&pass={1}", "value1", "value2");
                HttpWebRequest getRequest = (HttpWebRequest)WebRequest.Create(getUrl);

                getRequest.CookieContainer = new CookieContainer();
                //getRequest.CookieContainer.Add(cookies); //recover cookies First request
                getRequest.Method = WebRequestMethods.Http.Post;
                getRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
                getRequest.AllowWriteStreamBuffering = true;
                getRequest.ProtocolVersion = HttpVersion.Version11;
                getRequest.AllowAutoRedirect = true;
                getRequest.ContentType = "application/json";

                byte[] byteArray = Encoding.ASCII.GetBytes(postData);
                getRequest.ContentLength = byteArray.Length;
                Stream newStream = getRequest.GetRequestStream(); //open connection
                newStream.Write(byteArray, 0, byteArray.Length); // Send the data.
                newStream.Close();
                string sourceCode;
                HttpWebResponse getResponse = (HttpWebResponse)getRequest.GetResponse();
                using (StreamReader sr = new StreamReader(getResponse.GetResponseStream(), Encoding.GetEncoding(866)))//iso-8859-1 to ibm866
                {
                    sourceCode = sr.ReadToEnd();
                }
                schedules = JsonConvert.DeserializeObject<List<Para>>(sourceCode);
            }
            catch (Exception x)
            {
                DisplayAlert("Исключение", x.ToString(), "OK");
            }
        }
        private void GetSchedule(string text_button)
        {
            int i = 1;//временно
            string value = null;
            OpenServer();//обновление расписания // можно придумать другой способ обновления расписания
            int length = schedules.Count;
            schedule.Clear();
            MainStack.Children.Remove(listview);

            if (text_button == "Понедельник")
            {
                value = "monday";
            }
            else if (text_button == "Вторник")
            {
                value = "tuesday";
            }
            else if (text_button == "Среда")
            {
                value = "wednesday";
            }
            else if (text_button == "Четверг")
            {
                value = "thursday";
            }
            else if (text_button == "Пятница")
            {
                value = "friday";
            }

            for (int y = 0; y < length; y++)
            {
                if (schedules[y].day == value && schedules[y].week_color == week_color)
                {
                    schedule.Add(new Schedule
                    {
                        NumberPara = i.ToString(),
                        NameLesson = schedules[y].lesson_name,
                        NameTeacher = schedules[y].teacher,
                        NumberRoom = schedules[y].room
                    });
                    i++;
                }
            }

            listview.ItemsSource = schedule;
            listview.ItemTemplate = dataTemplate;
            MainStack.Children.Add(listview);
            Content = MainStack;
        }
        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            if (!Switch_color)
            {
                week_color = "green";
                Switch_color = true;
                label_week_color.Text = "сейчас зелёная неделя";
                label_week_color.TextColor = Color.Green;
            }
            else
            {
                week_color = "red";
                Switch_color = false;
                label_week_color.Text = "сейчас красная неделя";
                label_week_color.TextColor = Color.Red;
            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            string text_button = ((Button)sender).Text;
            GetSchedule(text_button);
        }

        private void Listview_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            listview.SelectedItem = null;
        }
    }
}