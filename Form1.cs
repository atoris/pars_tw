using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
namespace ParserTupperware_samara
{
    public partial class Form1 : Form
    {

        
        private Timer _timerWhois;
        private int _numS;


        private List<string> _lstDescription;
        private List<string> _lstName;
        private List<string> _lstPrice;
        private List<string> _lstMinDescription;
        private List<List<string>> _lstImage;
        private List<string> _linkPr;
        private List<string> _lstArticle;
        private List<string> _lstImg;
        private List<String> _lstNewPrice;
        private string _katName;
        private Int32 _numID=1;
        private List<string> _lstSk;
        private string text;

        public Form1()
        {
            InitializeComponent();

            dataGridView1.Rows.Clear();
            string[] row = new string[] { "ID", "Активен (0/1)", "Имя", "Категории (x,y,z...)", "Цена без налогов или Цена c налогами", "ID налога", "Оптовая цена", "Распродажа (0/1)", "Величина скидки", "Процент скидки", "Скидка действует от (гггг-мм-дд)", "Скидка действует до (гггг-мм-дд)", "Артикул №", "Артикул поставщика №", "Поставщик", "Производитель", "Штрихкод EAN13", "Штрихкод UPC", "Экологический налог", "Ширина", "Высота", "Глубина", "Вес", "Количество", "Минимальное количество", "Видимость", "Дополнительные расходы по доставке", "Единица измерения для стоимости за один товаров", "Цена", "Короткое описание", "Описание", "Метки (x,y,z...)", "Мета-заголовок", "Мета ключевые слова", "Мета описание", "ЧПУ", "Текст если товар в наличии", "Текст, если предварительный заказ разрешен", "Доступен для заказа (0= нет, 1 = да)", "Товар доступен с даты", "Дата создания товара", "Отображать цену (0 = нет, 1 = да)", "URL изображений (x,y,z...)", "Удалить существующие изображения (0 = нет, 1 = да)", "Свойство (Наименование:Значение:Позиция:Кастомизировано)", "Доступен только в режиме онлайн (0 = нет, 1 = да)", "Состояние", "Настраиваемый (0 = Нет, 1 = Да)", "Загружаемые файлы (0 = Нет, 1 = Да)", "Текстовые поля (0 = Нет, 1 = Да)", "Действия когда нет в наличии", "ID / Название магазина", "Расширенное Управление Запасами", "В зависимости от наличия", "Склад" };
            dataGridView1.Rows.Add(row);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                //String num = ;
                _numID = Int32.Parse(textBox2.Text);
                //_numID = Int32.Parse(num);
            }
            
            if (textBox3.Text!="")
            {
                _katName=textBox3.Text;
                //try{
                    getLinkP(textBox1.Text);
                //}catch{MessageBox.Show("Возможно вы не правильно ввели ссылку сайта");}
            }else{
                MessageBox.Show("Вы не ввели категорию");
            }
            
           
        }

        private void getLinkP(String url)
        {
            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load(url);
            _linkPr = new List<string>();
            _lstName = new List<string>();
            foreach (HtmlAgilityPack.HtmlNode node in doc.DocumentNode.SelectNodes("//*[contains(@class,'main-products')]/div/div[1]/a[@href]"))
            {
                //HtmlAgilityPack.HtmlAttribute att = node.Attributes["href"];
                string hrefValue = node.GetAttributeValue("href", string.Empty);
                _linkPr.Add(hrefValue);
            }

            foreach (HtmlAgilityPack.HtmlNode node2 in doc.DocumentNode.SelectNodes("//*[contains(@class,'main-products')]/div/div[2]/a[@href]"))
            {
                _lstName.Add(node2.InnerText);
            }
            
            parseInformation();
        }

        private void parseInformation()
        {
            _lstDescription = new List<string>();
            _lstPrice = new List<string>();
            _lstMinDescription = new List<string>();
            _lstImage = new List<List<string>>();
            _lstArticle = new List<string>();
            _lstNewPrice = new List<string>();
            _lstSk = new List<string>();
            text = "";
            if (_linkPr.Count > 0)
            {
                
                


                
                _numS = 0;
                _timerWhois = new Timer();
                _timerWhois.Interval = 50;
                _timerWhois.Tick += timerWhoisTick;
                _timerWhois.Enabled = true;
            }
            else
            {
                MessageBox.Show("нет ссылок");
            }
        }


        private void timerWhoisTick(object sender, EventArgs e)
        {
            
            //richTextBox1.Text = _numS.ToString();
            if (_numS < _linkPr.Count)
            {

                _lstImg = new List<string>();
                HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
                HtmlAgilityPack.HtmlDocument doc = web.Load(_linkPr[_numS]);
                //richTextBox1.Text = _numS.ToString();

                /**
                 * Описание товара
                 * */
                foreach (HtmlAgilityPack.HtmlNode node in doc.DocumentNode.SelectNodes("//*[contains(@id,'tab-description')]"))
                {

                    //richTextBox1.Text += node.InnerText;
                    _lstDescription.Add(node.InnerHtml);
                }
                /**
                 * Маленькое описание товаров
                 * */
                foreach (HtmlAgilityPack.HtmlNode node in doc.DocumentNode.SelectNodes("//*[contains(@class,'product-options')]/div[1]/span[3]"))
                {
                    if (node.InnerText != "Бонусные баллы:")
                    {
                        var text = "Производитель: Tupperware<br>Модель: " + node.InnerText;
                        _lstMinDescription.Add(text);
                        _lstArticle.Add(node.InnerText);
                    }
                    else
                    {
                        var text = "Производитель: Tupperware";
                        _lstMinDescription.Add(text);
                        _lstArticle.Add("");
                    }
                    
                }
                /**
                 * картинки
                 * */
                try
                {
                    foreach (HtmlAgilityPack.HtmlNode node in doc.DocumentNode.SelectNodes("//*[contains(@id,'product-gallery')]/a[@href]"))
                    {
                        string hrefValue = node.GetAttributeValue("href", string.Empty);
                        _lstImg.Add(hrefValue);
                    }
                }
                catch
                {
                    foreach (HtmlAgilityPack.HtmlNode node in doc.DocumentNode.SelectNodes("//*[contains(@class,'image-gallery')]/a[@href]"))
                    {
                        string hrefValue = node.GetAttributeValue("href", string.Empty);
                        richTextBox1.Text = hrefValue;
                        _lstImg.Add(hrefValue);
                    }
                }
                /**
                 * цена
                 * */
                try
                {
                    foreach (HtmlAgilityPack.HtmlNode node in doc.DocumentNode.SelectNodes("//*[contains(@class,'product-options')]/div[2]/span[1]"))
                    {
                        var match = Regex.Match(node.InnerText, @"[0-9][0-9]+(?:\.[0-9]*)?");
                        _lstNewPrice.Add("0");
                        _lstPrice.Add(match.Value);
                    }
                }
                catch
                {
                    foreach (HtmlAgilityPack.HtmlNode node in doc.DocumentNode.SelectNodes("//*[contains(@class,'product-options')]/div[3]/span[1]"))
                    {
                        var match = Regex.Match(node.InnerText, @"[0-9][0-9]+(?:\.[0-9]*)?");
                        _lstPrice.Add(match.Value);
                        //_lstNewPrice.Add(match.Value);
                    }
                    foreach (HtmlAgilityPack.HtmlNode node in doc.DocumentNode.SelectNodes("//*[contains(@class,'product-options')]/div[3]/span[2]"))
                    {
                        var match = Regex.Match(node.InnerText, @"[0-9][0-9]+(?:\.[0-9]*)?");
                        //_lstPrice.Add(node.InnerHtml);
                        _lstNewPrice.Add(match.Value);
                    }
                }
                var skl="";

                /**
                 * наличие
                 * */
                try
                {
                    foreach (HtmlAgilityPack.HtmlNode node in doc.DocumentNode.SelectNodes("//*[contains(@class,'product-options')]/div[1]/span[7]"))
                    {
                        skl = (node.InnerText == "Есть в наличии") ? "100" : "0";
                    }
                }
                catch {
                    try
                    {
                        foreach (HtmlAgilityPack.HtmlNode node in doc.DocumentNode.SelectNodes("//*[contains(@class,'product-options')]/div[1]/span[6]"))
                        {

                            skl = (node.InnerText == "Есть в наличии") ? "100" : "0";
                        }
                    }
                    catch { skl = "100"; }
                
                }

                //owl-item
                //journal-stock instock
                _lstImage.Add(_lstImg);
                string imageStr = string.Join(",", _lstImage[_numS].ToArray());
                
                //richTextBox1.Text = dogCsv;
               // _lstSk.Add((Convert.ToInt32(_lstNewPrice[_numS]) - Convert.ToInt32(_lstPrice[_numS])).ToString());




                string[] row = new string[] { _numID.ToString(), "1", _lstName[_numS], _katName, _lstPrice[_numS], "", "", "", "", "", "", "", _lstArticle[_numS], "", "", "Tupperware", "", "", "", "", "", "", "", skl, "", "", "", "", "", _lstMinDescription[_numS], _lstDescription[_numS].Replace(Environment.NewLine, " "), " ", "", "", "", "", "", "", "1", "", "", "1", imageStr, " ", "", "", "", "", "", "", "", "", "", "", "" };
                
                
                dataGridView1.Rows.Add(row);
                _numS++;
                _numID++;
            }
            else
            {
                _timerWhois.Enabled = false;
                _timerWhois.Stop();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string fileCSV = "";
            for (int i = 0; i < dataGridView1.RowCount - 1; i++)
            {
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    fileCSV += (dataGridView1[j, i].Value).ToString()+"|";
                }
                fileCSV += "\t\n";
            }
            StreamWriter wr = new StreamWriter(textBox3.Text+".csv", false, Encoding.UTF8);
            wr.Write(fileCSV);
            wr.Close();
        }

    }
}
