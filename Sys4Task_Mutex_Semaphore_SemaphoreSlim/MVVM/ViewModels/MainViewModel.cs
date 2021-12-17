using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Newtonsoft.Json;
using Sys4Task_Mutex_Semaphore_SemaphoreSlim.MVVM.Commands;
using Sys4Task_Mutex_Semaphore_SemaphoreSlim.MVVM.Models;

namespace Sys4Task_Mutex_Semaphore_SemaphoreSlim.MVVM.ViewModels 
{
    public class MainViewModel : BaseViewModel
    {
        #region Private Variable

        private List<string> _fileNameList;
        private DispatcherTimer _timer;
        private SemaphoreSlim _semaphoreSlim;
        private int _miliSeconds;
        private int _seconds;
        private int _minutes;
        private int _hours;
        private int _count;
        private static object _obj;

        #endregion

        #region Full Property

        private bool openOrClose;
        public bool OpenOrClose
        {
            get { return openOrClose; }
            set { openOrClose = value; OnPropertyChanged();}
        }



        private string time;
        public string Timer
        {
            get { return time; }
            set { time = value; OnPropertyChanged();}
        }

        #endregion

        #region Public Variable

        public ObservableCollection<ListBoxItem> Items { get; set; }

        #endregion

        #region Commands

        public ICommand StartButtonCommand { get; set; }

        #endregion


        public MainViewModel()
        {
            #region Set new data

            Items = new ObservableCollection<ListBoxItem>();
            _fileNameList = new List<string>();
            _timer = new DispatcherTimer();
            _semaphoreSlim = new SemaphoreSlim(4);
            _obj = new object();

            _fileNameList.Add("1.json");
            _fileNameList.Add("2.json");
            _fileNameList.Add("3.json");
            _fileNameList.Add("4.json");
            _fileNameList.Add("5.json");

            setData();

            _timer.Tick += TimerOnTick;
            Timer = "00:00:00:00";

            StartButtonCommand = new RelayCommand((o) => StartButtonOnClick());

            #endregion
        }


        private void TimerOnTick(object sender, EventArgs e)
        {
            if (_miliSeconds >= 60)
            {
                _miliSeconds = 0;
                _seconds++;
            }

            if (_seconds >= 60)
            {
                _seconds = 0;
                _minutes++;
            }

            if (_minutes >= 60)
            {
                _minutes = 0;
                _hours++;
            }

            _miliSeconds++;

            List<string> timer = new List<string>();
            if (_miliSeconds >= 0 && _miliSeconds <= 9) timer.Add($"0{_miliSeconds}");
            else if (_miliSeconds >= 10) timer.Add($"{_miliSeconds}");

            if (_seconds >= 0 && _seconds <= 9) timer.Add($"0{_seconds}");
            else if (_seconds >= 10) timer.Add($"{_seconds}");

            if (_minutes >= 0 && _minutes <= 9) timer.Add($"0{_minutes}");
            else if (_minutes >= 10) timer.Add($"{_minutes}");

            if (_hours >= 0 && _hours <= 9) timer.Add($"0{_hours}");
            else if (_hours >= 10) timer.Add($"{_hours}");

            Timer = $"{timer[3]}:{timer[2]}:{timer[1]}:{timer[0]}";
        }

        private void StartButtonOnClick()
        {
            _timer.Start();

            _hours = 0;
            _minutes = 0;
            _seconds = 0;
            _miliSeconds = 0;
            _count = 0;

            Items.Clear();

            if (!OpenOrClose)
            {
                ThreadPool.QueueUserWorkItem((o) => LockWithGetData());
            }
            else if (OpenOrClose)
            {
                ThreadPool.QueueUserWorkItem((o) => SemaphoreSlimWithGetData(_fileNameList[0]));
                ThreadPool.QueueUserWorkItem((o) => SemaphoreSlimWithGetData(_fileNameList[1]));
                ThreadPool.QueueUserWorkItem((o) => SemaphoreSlimWithGetData(_fileNameList[2]));
                ThreadPool.QueueUserWorkItem((o) => SemaphoreSlimWithGetData(_fileNameList[3]));
                ThreadPool.QueueUserWorkItem((o) => SemaphoreSlimWithGetData(_fileNameList[4]));
            }
        }

        private void SemaphoreSlimWithGetData(string fileName)
        {
            _semaphoreSlim.Wait();
            _count++;
            Car car = getDataDeSeralize(fileName);

            if (car != null)
            {
                ListBoxItem item = null;
                StackPanel panelMain = null;
                StackPanel mainInsidePanel = null;
                StackPanel panelInsidePanel1 = null;
                StackPanel panelInsidePanel2 = null;
                Image image = null;
                List<Label> labels = null;
                App.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    item = new ListBoxItem();
                    panelMain = new StackPanel() { Orientation = Orientation.Horizontal };
                    mainInsidePanel = new StackPanel() { Orientation = Orientation.Horizontal };
                    panelInsidePanel1 = new StackPanel();
                    panelInsidePanel2 = new StackPanel();
                    image = new Image()
                    {
                        Width = 150,
                        Height = 150,
                        Stretch = Stretch.Uniform,
                        Source = new BitmapImage(new Uri(car.ImagePath))
                    };
                    labels = new List<Label>()
                        {
                            new Label()
                            {
                                Content = "Model: ",
                                Foreground = new SolidColorBrush(Colors.White),
                                FontSize = 20
                            },
                            new Label()
                            {
                                Content = "Vendor: ",
                                Foreground = new SolidColorBrush(Colors.White),
                                FontSize = 20
                            },
                            new Label()
                            {
                                Content = "Year: ",
                                Foreground = new SolidColorBrush(Colors.White),
                                FontSize = 20
                            },
                            new Label()
                            {
                                Content = car.Model,
                                Foreground = new SolidColorBrush(Colors.White),
                                FontSize = 20
                            },
                            new Label()
                            {
                                Content = car.Vendor,
                                Foreground = new SolidColorBrush(Colors.White),
                                FontSize = 20
                            },
                            new Label()
                            {
                                Content = car.Year.ToString(),
                                Foreground = new SolidColorBrush(Colors.White),
                                FontSize = 20
                            },
                        };

                    panelInsidePanel1.Children.Add(labels[0]);
                    panelInsidePanel1.Children.Add(labels[1]);
                    panelInsidePanel1.Children.Add(labels[2]);
                    panelInsidePanel2.Children.Add(labels[3]);
                    panelInsidePanel2.Children.Add(labels[4]);
                    panelInsidePanel2.Children.Add(labels[5]);

                    mainInsidePanel.Children.Add(panelInsidePanel1);
                    mainInsidePanel.Children.Add(panelInsidePanel2);

                    panelMain.Children.Add(image);
                    panelMain.Children.Add(mainInsidePanel);
                    if (item != null)
                    {
                        item.Content = panelMain;
                        Items.Add(item);
                    }
                }));
                if (_count == 5)
                    _timer.Stop();

                Thread.Sleep(2000);
            }

            _semaphoreSlim.Release();
        }

        private void LockWithGetData()
        {
            lock (_obj)
            {
                string path = string.Empty;
                while (_count < 5)
                {
                    if (path == string.Empty)
                    {
                        path = _fileNameList[0];
                        _count++;
                    }
                    else if (path == _fileNameList[0])
                    {
                        path = _fileNameList[1];
                        _count++;
                    }
                    else if (path == _fileNameList[1])
                    {
                        path = _fileNameList[2];
                        _count++;
                    }
                    else if (path == _fileNameList[2])
                    {
                        path = _fileNameList[3];
                        _count++;
                    }
                    else if (path == _fileNameList[3])
                    {
                        path = _fileNameList[4];
                        _count++;
                    }

                    Car car = getDataDeSeralize(path);

                    if (car != null)
                    {
                        ListBoxItem item = null;
                        StackPanel panelMain = null;
                        StackPanel mainInsidePanel = null;
                        StackPanel panelInsidePanel1 = null;
                        StackPanel panelInsidePanel2 = null;
                        Image image = null;
                        List<Label> labels = null;
                        App.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            item = new ListBoxItem();
                            panelMain = new StackPanel() { Orientation = Orientation.Horizontal };
                            mainInsidePanel = new StackPanel() { Orientation = Orientation.Horizontal };
                            panelInsidePanel1 = new StackPanel();
                            panelInsidePanel2 = new StackPanel();
                            image = new Image()
                            {
                                Width = 150,
                                Height = 150,
                                Stretch = Stretch.Uniform,
                                Source = new BitmapImage(new Uri(car.ImagePath))
                            };
                            labels = new List<Label>()
                        {
                            new Label()
                            {
                                Content = "Model: ",
                                Foreground = new SolidColorBrush(Colors.White),
                                FontSize = 20
                            },
                            new Label()
                            {
                                Content = "Vendor: ",
                                Foreground = new SolidColorBrush(Colors.White),
                                FontSize = 20
                            },
                            new Label()
                            {
                                Content = "Year: ",
                                Foreground = new SolidColorBrush(Colors.White),
                                FontSize = 20
                            },
                            new Label()
                            {
                                Content = car.Model,
                                Foreground = new SolidColorBrush(Colors.White),
                                FontSize = 20
                            },
                            new Label()
                            {
                                Content = car.Vendor,
                                Foreground = new SolidColorBrush(Colors.White),
                                FontSize = 20
                            },
                            new Label()
                            {
                                Content = car.Year.ToString(),
                                Foreground = new SolidColorBrush(Colors.White),
                                FontSize = 20
                            },
                        };

                            panelInsidePanel1.Children.Add(labels[0]);
                            panelInsidePanel1.Children.Add(labels[1]);
                            panelInsidePanel1.Children.Add(labels[2]);
                            panelInsidePanel2.Children.Add(labels[3]);
                            panelInsidePanel2.Children.Add(labels[4]);
                            panelInsidePanel2.Children.Add(labels[5]);

                            mainInsidePanel.Children.Add(panelInsidePanel1);
                            mainInsidePanel.Children.Add(panelInsidePanel2);

                            panelMain.Children.Add(image);
                            panelMain.Children.Add(mainInsidePanel);
                            if (item != null)
                            {
                                item.Content = panelMain;
                                Items.Add(item);
                            }
                        }));
                        if (_count == 5)
                            _timer.Stop();
                        
                        Thread.Sleep(2000);
                    }
                }
            }
        }

        private Car getDataDeSeralize(string path)
        {
            using (StreamReader file = File.OpenText(path))
            using (JsonReader reader = new JsonTextReader(file))
            {
                JsonSerializer deSerializer = new JsonSerializer();
                return deSerializer.Deserialize<Car>(reader);
            }
        }

        private void writeInfoFile(string fileName, Car car)
        {
            using (StreamWriter file = File.CreateText(fileName))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, car);
            }
        }

        private void setData()
        {
            Car car;

            if (!File.Exists(_fileNameList[0]))
            {
                car = new Car()
                {
                    Model = "Mercedes-Benz",
                    Vendor = "Omar",
                    Year = 2021,
                    ImagePath = "https://cdn.jdpower.com/JDPA_2021%20Mercedes-Benz%20S-Class%20Black%20At%20Speed.jpg"
                };

                writeInfoFile(_fileNameList[0], car);
            }
            if (!File.Exists(_fileNameList[1]))
            {
                car = new Car()
                {
                    Model = "BMW",
                    Vendor = "Hesen",
                    Year = 2019,
                    ImagePath =
                        "https://hips.hearstapps.com/hmg-prod.s3.amazonaws.com/images/p90244780-highres-1560791619.jpg"
                };

                writeInfoFile(_fileNameList[1], car);
            }
            if (!File.Exists(_fileNameList[2]))
            {
                car = new Car()
                {
                    Model = "Volvo",
                    Vendor = "Ali",
                    Year = 2020,
                    ImagePath =
                        "https://platform.cstatic-images.com/large/in/v2/stock_photos/8c989675-af28-406a-b088-387d5f3a4dd9/acb6ec05-6939-4fcb-bb24-3e26767e67d6.png"
                };

                writeInfoFile(_fileNameList[2], car);
            }
            if (!File.Exists(_fileNameList[3]))
            {
                car = new Car()
                {
                    Model = "Tofash",
                    Vendor = "Mustafa",
                    Year = 2009,
                    ImagePath =
                        "https://i2.wp.com/www.klasikotom.com/wp-content/uploads/2021/06/1-Tofas-Sahin-Tarihcesi-Modelleri-Motor-Teknik-Ozellikleri-murat-131-sahin-kapakk.jpg?fit=1024%2C592&ssl=1"
                };

                writeInfoFile(_fileNameList[3], car);
            }
            if (!File.Exists(_fileNameList[4]))
            {
                car = new Car()
                {
                    Model = "Tesla",
                    Vendor = "Jhon",
                    Year = 2020,
                    ImagePath =
                        "https://hips.hearstapps.com/hmg-prod.s3.amazonaws.com/images/2020-tesla-model-x-mmp-1-1579127420.jpg"
                };

                writeInfoFile(_fileNameList[4], car);
            }
        }
    }
}