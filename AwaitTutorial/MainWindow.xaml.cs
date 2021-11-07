using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AwaitTutorial
{
    public delegate void ThisDelegate();
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        ////////////////////////////////////////////////////////////////////////////////////
        ///https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/async
        ///  async (C# Reference) In MSDN
        ////////////////////////////////////////////////////////////////////////////////////


        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // ExampleMethodAsync returns a Task<int>, which means that the method eventually produces
            // an int result. However, ExampleMothodAsync returns the Task<int> value as soon as it reaches an await.

            string testURL = InputTextBox.Text;

            ResultsTextBox.Text += "\n"; // The TextBox object from MainWindow.

            

            try
            {
                int length = await ExmpleMethodAsync(testURL);
                // Note that you could put "await ExampleMethodAsync()" in the next line where
                // "length" is, but due to when '+=' fetches the value of ResultsTextBox, you
                // would not see the global side effect of ExampleMethodAsync setting the text.
                ResultsTextBox.Text += String.Format("Lengh: {0:N0}\n", length);

                // 위에서 말한 것은 async 안의 텍스트박스 추가 동작이 씹힌다는 것.  이는 메인 윈도우의 스레드에 분기된 async함수의 스레드가 간섭하지 못하는 이유이다.

                Task<string> returnedMessage = ShowTodaysInfoAsync(); // 위에서 한줄로 나타낸 코드는 이렇게 풀어쓸 수 있다.
                string msg = await returnedMessage;
                ResultsTextBox.Text += msg + Environment.NewLine;
                //ResultsTextBox.Text += await ShowTodaysInfoAsync() + Environment.NewLine;
            }
            catch (Exception ex)
            {
                // Process the exception if one occurs.
                Console.WriteLine(ex);
            }
        }

        public async Task<int> ExmpleMethodAsync(string url)
        {
            var httpClient = new HttpClient(); // from System.Net.Http
            int exampleInt = (await httpClient.GetStringAsync( url )).Length;
            ResultsTextBox.Text += "Preparing to finish ExampleMethodAsync.\n";

            await Task.Delay(2000); // 강제 딜레이
            // After the following return statement, any method that's awaiting ExampleMethodAsync
            // - 본 예에서는 StartButton_Click - can get the integer result.
            return exampleInt;
        }

        public async Task<string> ShowTodaysInfoAsync()
        {
            ResultsTextBox.Text += "\nWe are now calculating your today's leisure time.\n";
            await Task.Delay(1000); // 강제 딜레이
            string message =
                $"Today is {DateTime.Today:D}\n" +
                "Today's hours of leisure: " +
                $"{await GetLeisureHoursAsync()}";

            return message;
            //ResultsTextBox.Text += "We are now calculating your today's leisure time.\n"; // 1st TRY: 씹힘

            // 2nd TRY: System.InvalidOperationException: '다른 스레드가 이 개체를 소유하고 있어 호출한 스레드가 해당 개체에 액세스할 수 없습니다.'
            //var t =  Task.Run(() => ResultsTextBox.Text += "We are now calculating your today's leisure time.\n");
            //t.Wait();

            //// 3rd TRY:
            //Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            //{
            //    ResultsTextBox.Text += "We are now calculating your today's leisure time.\n";
            //}));

            // 4th TRY:            
            //this.Dispatcher.Invoke(DispatcherPriority.Background, (ThisDelegate)delegate()
            //{
            //    ResultsTextBox.Text += "We are now calculating your today's leisure time.\n";
            //});

            // 5th TRY:  이제 나오긴 나오는데 다 끝나고 출력됨
            //_ = this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            //  {
            //      ResultsTextBox.Text += "We are now calculating your today's leisure time.\n";
            //  }));

            // 6th TRY: 다시실패
            //int leisureHour = await GetLeisureHoursAsync();
            //return await Task.Run(() =>
            // {
            //     string message =
            //         $"Today is {DateTime.Today:D}\n" +
            //         $"Today's hours of leisure: {leisureHour}";

            //     return message;
            // });
        }

        async Task<int> GetLeisureHoursAsync()
        {
            DayOfWeek today = await Task.FromResult(DateTime.Now.DayOfWeek);
            
            int leisureHours = 
                today is DayOfWeek.Saturday || today is DayOfWeek.Sunday
                ? 16 : 5; // 오늘이 토/일 주말이면 여가시간은 16시간. 주중이면 5시간.
            return leisureHours;         
        }
    }
}
