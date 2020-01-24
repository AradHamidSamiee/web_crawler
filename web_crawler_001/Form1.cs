using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
// selenium stuff (+ needed libraries)
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Xml;
using System.Net;
using static System.Windows.Forms.LinkLabel;

namespace web_crawler_001
{
    public partial class Form1 : Form
    {
        private int seconds_passed;

        public Form1()
        {
            InitializeComponent();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        int func_vis = 0;
        private void button2_Click(object sender, EventArgs e)
        {
            if (func_vis == 0)
            {
                panel2.Visible = true;
                func_vis++;
            }
            else if (func_vis == 1)
            {
                panel2.Visible = false;
                func_vis--;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            listBox1.Items.Add(" - this session will terminate in 3");
            timer1.Start();
        }
        int seconds_left = 2;
        private void timer1_Tick(object sender, EventArgs e)
        {
            seconds_passed++;
            listBox1.Items.Add(" - this session will terminate in " + seconds_left);
            seconds_left--;
            if (seconds_passed == 3)
            {
                Application.Exit();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            listBox1.Items.Add(" - driver chrome : new object about to get created");
            listBox1.Items.Add(" - driver chrome : no sand-box");
            listBox1.Items.Add(" - driver chrome : no options as input for chrome web browser are set");
            // Initialize the Chrome Driver
            using (var driver = new ChromeDriver())
            {
                // Go to the home page
                driver.Navigate().GoToUrl("http://testing-ground.scraping.pro/login");
                listBox1.Items.Add(" - chrome started successfuly ");

                // Get the page elements
                var userNameField = driver.FindElementById("usr");
                var userPasswordField = driver.FindElementById("pwd");
                var loginButton = driver.FindElementByXPath("//input[@value='Login']");

                // Type user name and password
                userNameField.SendKeys("admin");
                listBox1.Items.Add(" - username: 'admin' >>> sent to the server ");
                userPasswordField.SendKeys("12345");
                listBox1.Items.Add(" - password: '12345' >>> sent to the server ");

                // and click the login button
                loginButton.Click();
                listBox1.Items.Add(" - login_button: clicked ");

                // Extract the text and save it into result.txt
                var result_testsite = driver.FindElementByXPath("//div[@id='case_login']/h3").Text;
                File.WriteAllText("result_testsite.txt", result_testsite);

                // Take a screenshot and save it into screen.png
                driver.GetScreenshot().SaveAsFile(@"screen_testsite.png"/*, ImageFormat.Png*/);
                listBox1.Items.Add(" - a screen shot is taken ");
            }
        }

        int visible_panel = 0;
        private void button7_Click_1(object sender, EventArgs e)
        {
            if (visible_panel == 0)
            {
                panel1.Visible = Enabled;
                visible_panel++;
            }
            else if (visible_panel == 1)
            {
                panel1.Visible = false;
                if (func_vis == 1)
                {
                    panel2.Visible = false;
                    func_vis--;
                }
                visible_panel--;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");
            // Initialize the Chrome Driver
            using (var driver = new ChromeDriver(chromeOptions))
            {
                // Go to the home page
                driver.Navigate().GoToUrl(textBox1.Text);
                listBox1.Items.Add(" - browser: 'chrome' started successfuly ");

                // radioButton 1 - screenshot
                if (checkBox1.CheckState == CheckState.Checked)
                {
                    driver.GetScreenshot().SaveAsFile(textBox2.Text/*, ImageFormat.Png*/);
                    listBox1.Items.Add(" - a screen shot is taken ");
                }

                // radioButton 3 - link extraction
                if (checkBox3.CheckState == CheckState.Checked)
                {
                    IList<IWebElement> linksss = driver.FindElements(By.TagName("a"));
                    var numberoflinks = driver.FindElements(By.TagName("a"));
                    listBox1.Items.Add(" - there are '" + numberoflinks.Count() + "' links on '" + textBox1.Text + "'");
                    // how many links??? oh, right
                    int quanta = Convert.ToInt32(textBox5.Text);
                    for (int i = 1; i <= quanta; i++) //if i=numberoflinks.Count(), stops after maintaning web connection for 60secs due to politeness
                    {
                        IList<IWebElement> newLinks = driver.FindElements(By.TagName("a"));
                        listBox1.Items.Add(" - " + i + " : " + newLinks[i].GetAttribute("href").ToString());
                        File.AppendAllText(textBox4.Text, newLinks[i].GetAttribute("href") + Environment.NewLine);
                    }
                    // demo : crawling cycle - begins--------------------------------------------------------------------------------------------------------
                    /*
                    using (StreamReader sr = File.OpenText(textBox4.Text))
                    {
                        int i = 0;
                        string s = String.Empty;
                        while ((s = sr.ReadLine()) != null)
                        {
                            i++;
                            listBox1.Items.Add(i);
                        }
                    }*/
                    // actual :
                    using (StreamReader sr = File.OpenText(textBox4.Text))
                    {
                        int m = 20; // handling connection duration
                        string s = String.Empty;
                        while (m != 0 && (s = sr.ReadLine()) != null)
                        {
                            m--;
                            driver.Navigate().GoToUrl(s);
                            if (checkBox3.CheckState == CheckState.Checked)
                            {
                                linksss = driver.FindElements(By.TagName("a"));
                                numberoflinks = driver.FindElements(By.TagName("a"));
                                listBox1.Items.Add(" - there are '" + numberoflinks.Count() + "' links on '" + s + "'");
                                quanta = Convert.ToInt32(textBox5.Text);
                                for (int i = 1; i <= quanta; i++)
                                {
                                    IList<IWebElement> newLinks = driver.FindElements(By.TagName("a"));
                                    listBox1.Items.Add(" - " + i + " : " + newLinks[i].GetAttribute("href").ToString());
                                    File.AppendAllText(textBox3.Text, newLinks[i].GetAttribute("href") + Environment.NewLine);
                                }
                            }
                        }
                    }
                }
            }
            listBox1.Items.Add(" - browser: 'chrome' is now closed ");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This program is an early access crawler\nI originally created as a project for school\n\nCredits: Arad HamidSamiee\nContacts: asam235711@gmail.com");
        }
    }
}
