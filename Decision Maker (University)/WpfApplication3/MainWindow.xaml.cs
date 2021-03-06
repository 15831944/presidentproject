﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Threading;
using System.Diagnostics;
using System.Xml;
using WpfZhihui;
using System.Runtime.InteropServices;

namespace WpfApplication3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
                //调用系统音量
        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, UInt32 dwFlags, UInt32 dwExtraInfo);

        [DllImport("user32.dll")]
        static extern Byte MapVirtualKey(UInt32 uCode, UInt32 uMapType);
        private const byte VK_VOLUME_MUTE = 0xAD;
        private const byte VK_VOLUME_DOWN = 0xAE;
        private const byte VK_VOLUME_UP = 0xAF;
        private const UInt32 KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const UInt32 KEYEVENTF_KEYUP = 0x0002;

		string strPATH;
        private List<NewsItem> items = new List<NewsItem>();
        //-------------------------------------------------------------
        public MainWindow()
        {
            InitializeComponent();
            DirectoryInfo di;
            di = new DirectoryInfo(System.Environment.CurrentDirectory);
            strPATH = di.Parent.Parent.FullName;
            wb_weibo.Navigate(new Uri(strPATH + @"/html/weibo.htm"));
            LoadRss();
            DispatcherTimer timer = new DispatcherTimer();
            //设置间隔1秒
            timer.Interval = new TimeSpan(0, 0, 1);
            //创建事件处理
            timer.Tick += new EventHandler(timer_Tick);
            //开始计时
            timer.Start();


            Home_Click(new object(), new RoutedEventArgs());
        }
        void timer_Tick(object sender, EventArgs e)
        {
            DateHelper datehelper = new DateHelper();
            labe_time.Content = datehelper.getTime();
            label_day.Content = datehelper.getDay();
            label_dayofweek.Content = datehelper.getDayofWeek();
            label_month.Content = datehelper.getMonth();
            label_dayofweek_zh.Content = datehelper.getDayofWeekzh();
            label_month.Content = datehelper.getMonth();
            label_month_zh.Content = datehelper.getMonthzh();
        }

        private void imageTVCover_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Dongfangweishi.IsSelected = true;
        }
        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string ix = (e.AddedItems[0].ToString().Split(' '))[1];
            if (ix == "东方卫视")
            {
                imageTVCover.Visibility = Visibility.Hidden;
                WebBrowserTV.Visibility = Visibility.Visible;
                WebBrowserTV.Navigate(new Uri(strPATH + @"/html/zhiboDongfangweishi.htm", UriKind.RelativeOrAbsolute));

            }
            else if (ix == "CCTV新闻")
            {
                imageTVCover.Visibility = Visibility.Hidden;
                WebBrowserTV.Visibility = Visibility.Visible;

                DirectoryInfo di;
                di = new DirectoryInfo(System.Environment.CurrentDirectory);
                string strpath = di.Parent.Parent.FullName;
                WebBrowserTV.Navigate(new Uri(strpath + @"/html/zhiboCCTV13.htm", UriKind.RelativeOrAbsolute));
            }
            else if (ix == "CCTV体育")
            {
                imageTVCover.Visibility = Visibility.Hidden;
                WebBrowserTV.Visibility = Visibility.Visible;
                WebBrowserTV.Navigate(new Uri(strPATH + @"/html/zhiboCCTV5.htm", UriKind.RelativeOrAbsolute));
            }
            else if (ix == "五星体育")
            {
                imageTVCover.Visibility = Visibility.Hidden;
                WebBrowserTV.Visibility = Visibility.Visible;
                WebBrowserTV.Navigate(new Uri(strPATH + @"/html/zhiboWuxingtiyu.htm", UriKind.RelativeOrAbsolute));
            }

        }

        public void LoadRss()
        {
            string[] rsspath ={ "http://news.tongji.edu.cn/rss.php?classid=6",
                                   //"http://www.xinhuanet.com/politics/news_politics.xml",
                                   "http://news.tongji.edu.cn/rss.php?classid=7",
                                   "http://news.tongji.edu.cn/rss.php?classid=8"
                                   ,"http://news.tongji.edu.cn/rss.php?classid=10"
                              };//RSS地址
            string[] channeltitle ={"同济大学综合新闻",
                                   //"新华时政",
                                   "同济大学教学新闻",
                                   "同济大学科研新闻"
                                   ,"同济大学外事新闻"
                                   };
            string[] channellink ={"http://news.tongji.edu.cn/classid-176.html",
                                   //"http://www.xinhuanet.com/politics/xw.htm",
                                   "http://news.tongji.edu.cn/classid-176.html",
                                   "http://news.tongji.edu.cn/classid-176.html"
                                   ,"http://news.tongji.edu.cn/classid-176.html"
                                  };
            string[] logopath = {
                                "/Images/logo_tongji.jpg",
                                //"/Images/新华1.jpg",
                                "/Images/logo_tongji.jpg",
                                "/Images/logo_tongji.jpg"
                                ,"/Images/logo_tongji.jpg"
                                };
            for (int cid = 0; cid < 4; cid++)
            {
                XmlDocument doc = new XmlDocument();//创建文档对象
                try
                {
                    doc.Load(rsspath[cid]);//加载XML 包括HTTP：// 和本地
                    XmlNodeList list = doc.GetElementsByTagName("item"); //获得项   
                    XmlNode node = list.Item(0);//
                    NewsItem item = new NewsItem();
                    item = getItem((XmlElement)node);
                    item.ChannelLink = channellink[cid];
                    item.ChannelTitle = channeltitle[cid];
                    item.LogoPath = logopath[cid];
                    //加入list
                    items.Add(item);
                }
                catch (Exception)
                {
                    //异常处理
                }
                //初始化Rss 
                
            }

            //添加绑定操作
            this.Rsslist.ItemsSource = items;
        }
        private NewsItem getItem(XmlElement ele)
        {
            NewsItem item = new NewsItem();
            item.Title = ele.GetElementsByTagName("title")[0].InnerText;//获得标题
            item.Link = ele.GetElementsByTagName("link")[0].InnerText;//获得联接
            string des = ele.GetElementsByTagName("description")[0].InnerText;//获得简介
            if (des.Length > 80)
            {
                des = des.Substring(0, 80) + "……";
            }
            item.Description = des;
            //item.PubDate = ele.GetElementsByTagName("pubDate")[0].InnerText;//获得发布日期
            return item;
        }

        private void weibo_topic_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string topic = weibo_topic.Text;
                WeiboHelper weibohelper = new WeiboHelper();
                weibohelper.settopic(topic);
                wb_weibo.Navigate(new Uri(strPATH + @"/bin/Debug/weibo.htm"));
            }
        }
        private void weibo_topic_GotMouseCapture(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // TODO: Add event handler implementation here.
            weibo_topic.Text = "";
        }
        private void weibo_search_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO: Add event handler implementation here.
            string topic = weibo_topic.Text;
            WeiboHelper weibohelper = new WeiboHelper();
            weibohelper.settopic(topic);
            wb_weibo.Navigate(new Uri(strPATH + @"/bin/Debug/weibo.htm"));
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;

        }

        private void Home_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO: Add event handler implementation here.
			HideMenu();
			UnselectMenus();
			VisibleMenuTitle();
            HomePage m_HomePage = new HomePage();
            FrameMiddleContent.Navigate(m_HomePage);
        }
		
        private void Btn_Schedule_Click(object sender, RoutedEventArgs e)
        {
            DailyManagement.Schedule m_Schedule = new DailyManagement.Schedule();
            FrameMiddleContent.Navigate(m_Schedule);
        }

		private void HideMenu()
		{
                menu1.Visibility = System.Windows.Visibility.Collapsed;
                menu2.Visibility = System.Windows.Visibility.Collapsed;
                menu3.Visibility = System.Windows.Visibility.Collapsed;
                menu4.Visibility = System.Windows.Visibility.Collapsed;
                menu6.Visibility = System.Windows.Visibility.Collapsed;
                menu7.Visibility = System.Windows.Visibility.Collapsed;
                menu5.Visibility = System.Windows.Visibility.Collapsed;
				menu8.Visibility = System.Windows.Visibility.Collapsed;    
		}
		private void VisibleMenuTitle()
		{
			    menutitle1.Visibility = System.Windows.Visibility.Visible;
                menutitle2.Visibility = System.Windows.Visibility.Visible;
                menutitle3.Visibility = System.Windows.Visibility.Visible;
                menutitle4.Visibility = System.Windows.Visibility.Visible;
                menutitle6.Visibility = System.Windows.Visibility.Visible;
                menutitle7.Visibility = System.Windows.Visibility.Visible;
                menutitle5.Visibility = System.Windows.Visibility.Visible;
				menutitle8.Visibility = System.Windows.Visibility.Visible;    

		}
		
		private void HideMenuTitle()
		{
			    menutitle1.Visibility = System.Windows.Visibility.Collapsed;
                menutitle2.Visibility = System.Windows.Visibility.Collapsed;
                menutitle3.Visibility = System.Windows.Visibility.Collapsed;
                menutitle4.Visibility = System.Windows.Visibility.Collapsed;
                menutitle6.Visibility = System.Windows.Visibility.Collapsed;
                menutitle7.Visibility = System.Windows.Visibility.Collapsed;
                menutitle5.Visibility = System.Windows.Visibility.Collapsed;
				menutitle8.Visibility = System.Windows.Visibility.Collapsed;    

		}
		private void UnselectMenus()
		{
			menutitle1.IsSelected=false;
			menutitle2.IsSelected=false;
			menutitle3.IsSelected=false;
			menutitle4.IsSelected=false;
			menutitle5.IsSelected=false;
			menutitle6.IsSelected=false;
			menutitle7.IsSelected=false;
			menutitle8.IsSelected=false;			
		}
		        
        //private void menutitle1_Selected(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    // TODO: Add event handler implementation here.
        //    HideMenu();
        //        HideMenuTitle();
        //        menu1.Visibility = System.Windows.Visibility.Visible;
        //        menutitle1.Visibility = System.Windows.Visibility.Visible;
        //}

				
        //private void menutitle2_Selected(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    // TODO: Add event handler implementation here.
        //    HideMenu();
        //        HideMenuTitle();
        //        menu2.Visibility = System.Windows.Visibility.Visible;
        //        menutitle2.Visibility = System.Windows.Visibility.Visible;

        //}


        //private void menutitle3_Selected(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    // TODO: Add event handler implementation here.
        //    HideMenu();
        //        HideMenuTitle();
        //        menu3.Visibility = System.Windows.Visibility.Visible;
        //        menutitle3.Visibility = System.Windows.Visibility.Visible;
        //}
				        
        //private void menutitle4_Selected(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    // TODO: Add event handler implementation here.
        //    HideMenu();
        //        HideMenuTitle();
        //        menu4.Visibility = System.Windows.Visibility.Visible;
        //        menutitle4.Visibility = System.Windows.Visibility.Visible;
        //}
		        
        //private void menutitle5_Selected(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    // TODO: Add event handler implementation here.
        //    HideMenu();
        //        HideMenuTitle();
        //        menu5.Visibility = System.Windows.Visibility.Visible;
        //        menutitle5.Visibility = System.Windows.Visibility.Visible;
        //}
		        
        //private void menutitle6_Selected(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    // TODO: Add event handler implementation here.
        //    HideMenu();
        //        HideMenuTitle();
        //        menu6.Visibility = System.Windows.Visibility.Visible;
        //        menutitle6.Visibility = System.Windows.Visibility.Visible;
        //}
		        
        //private void menutitle7_PreviewMouseDown(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    // TODO: Add event handler implementation here.
        //    HideMenu();
        //        HideMenuTitle();
        //        menu7.Visibility = System.Windows.Visibility.Visible;
        //        menutitle7.Visibility = System.Windows.Visibility.Visible;

        //}
		        
        //private void menutitle8_PreviewMouseDown(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    // TODO: Add event handler implementation here.
        //    HideMenu();
        //        HideMenuTitle();
        //        menu8.Visibility = System.Windows.Visibility.Visible;
        //        menutitle8.Visibility = System.Windows.Visibility.Visible;
        //}



        private void menu6_3_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			// TODO: Add event handler implementation here.
			PageShiGuZaiHai m_PageShiGuZaiHai = new PageShiGuZaiHai();
            FrameMiddleContent.Navigate(m_PageShiGuZaiHai);

		}

		private void menu7_1_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			// TODO: Add event handler implementation here.
			FeedBack.PageTeacherStudentFeedback m_PageTeacherStudentFeedback = new FeedBack.PageTeacherStudentFeedback(1);
            FrameMiddleContent.Navigate(m_PageTeacherStudentFeedback);
			
			
		}

		private void menu4_2_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			// TODO: Add event handler implementation here.
			Communicate.PageDocIssue m_PageDocIssue = new Communicate.PageDocIssue();
            FrameMiddleContent.Navigate(m_PageDocIssue);
		}

		private void menu8_1_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			// TODO: Add event handler implementation here.
			LargeProject.PageProject m_PageProject = new LargeProject.PageProject(1);
            FrameMiddleContent.Navigate(m_PageProject);

		}

		private void menu5_1_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			// TODO: Add event handler implementation here.
			
            Case.PageCase m_PageCase = new Case.PageCase();
            FrameMiddleContent.Navigate(m_PageCase);
		}

		private void menu4_3_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			// TODO: Add event handler implementation here.
            DailyManagement.VideoConferencing m_VideoConferencing = new DailyManagement.VideoConferencing();
            FrameMiddleContent.Navigate(m_VideoConferencing);
		}

		private void menu4_1_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			// TODO: Add event handler implementation here.
			DailyManagement.Schedule m_Schedule = new DailyManagement.Schedule();
            FrameMiddleContent.Navigate(m_Schedule);

		}

        private void menu2_2_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ResourcesAndManpower.PageOrganizationStructure m_OrganizationStructure = new ResourcesAndManpower.PageOrganizationStructure();
            FrameMiddleContent.Navigate(m_OrganizationStructure);
        }

        private void menu2_1_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ResourcesAndManpower.PageSpatialResource m_Page = new ResourcesAndManpower.PageSpatialResource();
            FrameMiddleContent.Navigate(m_Page);
        }

        private void menu3_1_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
			Communicate.PageDocIssue m_PageDocIssue = new Communicate.PageDocIssue();
            FrameMiddleContent.Navigate(m_PageDocIssue);
        }
        private void menu3_2_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Communicate.PageBureauCommunicate m_PageBureauDoc = new Communicate.PageBureauCommunicate();
            FrameMiddleContent.Navigate(m_PageBureauDoc);
        }

        private void menu7_2_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            FeedBack.PageTeacherStudentFeedback m_PageTeacherStudentFeedback = new FeedBack.PageTeacherStudentFeedback(2);
            FrameMiddleContent.Navigate(m_PageTeacherStudentFeedback);
        }

        private void menu7_3_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            FeedBack.PageTeacherStudentFeedback m_PageTeacherStudentFeedback = new FeedBack.PageTeacherStudentFeedback(3);
            FrameMiddleContent.Navigate(m_PageTeacherStudentFeedback);
        }

        private void menu6_2_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PageSheHuiAnQuan page = new PageSheHuiAnQuan();
            FrameMiddleContent.Navigate(page);
        }

        private void menu6_1_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PageZiRanZaiHai page = new PageZiRanZaiHai();
            FrameMiddleContent.Navigate(page);
        }

        private void menu6_4_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PageGongGongWeiSheng page = new PageGongGongWeiSheng();
            FrameMiddleContent.Navigate(page);
        }

        private void menu8_2_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //LargeProject.PageProjectGym m_PageProject = new LargeProject.PageProjectGym();
            //FrameMiddleContent.Navigate(m_PageProject);

            LargeProject.PageProject m_PageProject = new LargeProject.PageProject(2);
            FrameMiddleContent.Navigate(m_PageProject);
        }

        private void menu8_3_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //LargeProject.PageProjectOuter m_PageProject = new LargeProject.PageProjectOuter();
            //FrameMiddleContent.Navigate(m_PageProject);
            
			LargeProject.PageProject m_PageProject = new LargeProject.PageProject(3);
            FrameMiddleContent.Navigate(m_PageProject);
        }
        private void menu8_5_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

            LargeProject.PageProject m_PageProject = new LargeProject.PageProject(5);
            FrameMiddleContent.Navigate(m_PageProject);
        }

        private void menu8_4_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

            LargeProject.PageProject m_PageProject = new LargeProject.PageProject(4);
            FrameMiddleContent.Navigate(m_PageProject);
        }

        private void menu8_6_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

            LargeProject.PageProject m_PageProject = new LargeProject.PageProject(6);
            FrameMiddleContent.Navigate(m_PageProject);
        }

        private void menutitle1_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (menu1.Visibility == System.Windows.Visibility.Visible)
            {
                HideMenu();
                UnselectMenus();
                VisibleMenuTitle();
                HomePage m_HomePage = new HomePage();
                FrameMiddleContent.Navigate(m_HomePage);
                //
                menu1_1.IsSelected = false;
                menu1_2.IsSelected = false;
                menu1_3.IsSelected = false;
                menu1_4.IsSelected = false;
            }
            else
            {
                HideMenu();
                HideMenuTitle();
                menu1.Visibility = System.Windows.Visibility.Visible;
                menutitle1.Visibility = System.Windows.Visibility.Visible;
                //点击后默认显示综合指数
                Development_performance.ComprehensiveIndex ComprehensiveI = new Development_performance.ComprehensiveIndex(this);
                FrameMiddleContent.Navigate(ComprehensiveI);
            }
        }
        private void TalentC(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // TODO: Add event handler implementation here.
            Development_performance.TalentCultivation TalentC = new Development_performance.TalentCultivation(1,this);
            FrameMiddleContent.Navigate(TalentC);

        }
        private void ScientificR(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // TODO: Add event handler implementation here.
            Development_performance.ScientificResearch ScientificR = new Development_performance.ScientificResearch(1,this);
            FrameMiddleContent.Navigate(ScientificR);

        }

        private void ComprehensiveR(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // TODO: Add event handler implementation here.
            Development_performance.ComprehensiveReputation ComprehensiveR = new Development_performance.ComprehensiveReputation(1,this);
            FrameMiddleContent.Navigate(ComprehensiveR);

        }
        private void GreenC(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // TODO: Add event handler implementation here.
            Development_performance.GreenCampus GreenC = new Development_performance.GreenCampus(1,this);
            FrameMiddleContent.Navigate(GreenC);

        }
		private void menutitle2_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (menu2.Visibility == System.Windows.Visibility.Visible)
            {
                HideMenu();
                UnselectMenus();
                VisibleMenuTitle();
            }
            else
            {
                HideMenu();
                HideMenuTitle();
                menu2.Visibility = System.Windows.Visibility.Visible;
                menutitle2.Visibility = System.Windows.Visibility.Visible;
            }
        }
		
		private void menutitle3_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (menu3.Visibility == System.Windows.Visibility.Visible)
            {
                HideMenu();
                UnselectMenus();
                VisibleMenuTitle();
            }
            else
            {
                HideMenu();
                HideMenuTitle();
                menu3.Visibility = System.Windows.Visibility.Visible;
                menutitle3.Visibility = System.Windows.Visibility.Visible;
            }
        }
		
		private void menutitle4_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (menu4.Visibility == System.Windows.Visibility.Visible)
            {
                HideMenu();
                UnselectMenus();
                VisibleMenuTitle();
            }
            else
            {
                HideMenu();
                HideMenuTitle();
                menu4.Visibility = System.Windows.Visibility.Visible;
                menutitle4.Visibility = System.Windows.Visibility.Visible;
            }
        }
		
		private void menutitle5_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (menu5.Visibility == System.Windows.Visibility.Visible)
            {
                HideMenu();
                UnselectMenus();
                VisibleMenuTitle();
            }
            else
            {
                HideMenu();
                HideMenuTitle();
                menu5.Visibility = System.Windows.Visibility.Visible;
                menutitle5.Visibility = System.Windows.Visibility.Visible;
            }
        }
		
		private void menutitle6_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (menu6.Visibility == System.Windows.Visibility.Visible)
            {
                HideMenu();
                UnselectMenus();
                VisibleMenuTitle();
            }
            else
            {
                HideMenu();
                HideMenuTitle();
                menu6.Visibility = System.Windows.Visibility.Visible;
                menutitle6.Visibility = System.Windows.Visibility.Visible;
            }
        }
		
		private void menutitle7_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (menu7.Visibility == System.Windows.Visibility.Visible)
            {
                HideMenu();
                UnselectMenus();
                VisibleMenuTitle();
            }
            else
            {
                HideMenu();
                HideMenuTitle();
                menu7.Visibility = System.Windows.Visibility.Visible;
                menutitle7.Visibility = System.Windows.Visibility.Visible;
            }
        }
		
		private void menutitle8_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (menu8.Visibility == System.Windows.Visibility.Visible)
            {
                HideMenu();
                UnselectMenus();
                VisibleMenuTitle();
            }
            else
            {
                HideMenu();
                HideMenuTitle();
                menu8.Visibility = System.Windows.Visibility.Visible;
                menutitle8.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            WebBrowserTV.Dispose();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
		
        private void ckbMute_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            keybd_event(VK_VOLUME_MUTE, MapVirtualKey(VK_VOLUME_MUTE, 0), KEYEVENTF_EXTENDEDKEY, 0);
            keybd_event(VK_VOLUME_MUTE, MapVirtualKey(VK_VOLUME_MUTE, 0), KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
		}

        private void ckbMute_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            keybd_event(VK_VOLUME_MUTE, MapVirtualKey(VK_VOLUME_MUTE, 0), KEYEVENTF_EXTENDEDKEY, 0);
            keybd_event(VK_VOLUME_MUTE, MapVirtualKey(VK_VOLUME_MUTE, 0), KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
        }



 
		
    }
}
