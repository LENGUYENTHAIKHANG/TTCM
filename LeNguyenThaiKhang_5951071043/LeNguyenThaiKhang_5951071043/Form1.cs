using xNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Win32;
using System.Diagnostics;

namespace LeNguyenThaiKhang_5951071043
{
    public partial class TKBrowser : Form
    {
        
        String settingsXml = "settings.xml", historyXml = "history.xml";
        List<String> urls = new List<String>();
        string adress, name;
        String homePage;
        XmlDocument settings = new XmlDocument();
        public static String favXml = "favorits.xml", linksXml = "links.xml";
        CultureInfo currentCulture;
        RegistryKey reg_key;
        public TKBrowser()
        {
            InitializeComponent();
            currentCulture = CultureInfo.CurrentCulture;
            



          
        }
     

        private void TKBrowser_Load(object sender, EventArgs e)
        {
           
            getCurrentBrowser().Navigate("https://www.google.com/webhp");
            this.toolStripStatusLabel1.Text = "Done";
            comboBox1.SelectedItem = comboBox1.Items[0];
            setVisibility();
            

        }
        private void setVisibility()
        {
            if (!File.Exists(settingsXml))
            {
                XmlElement r = settings.CreateElement("settings");
                settings.AppendChild(r);
                XmlElement el;

                el = settings.CreateElement("menuBar");
                el.SetAttribute("visible", "True");
                r.AppendChild(el);

                el = settings.CreateElement("adrBar");
                el.SetAttribute("visible", "True");
                r.AppendChild(el);

                el = settings.CreateElement("linkBar");
                el.SetAttribute("visible", "True");
                r.AppendChild(el);

                el = settings.CreateElement("favoritesPanel");
                el.SetAttribute("visible", "True");
                r.AppendChild(el);

                el = settings.CreateElement("SplashScreen");
                el.SetAttribute("checked", "True");
                r.AppendChild(el);

                el = settings.CreateElement("homepage");
                el.InnerText = "about:blank";
                r.AppendChild(el);

                el = settings.CreateElement("dropdown");
                el.InnerText = "15";
                r.AppendChild(el);
            }
            else
            {
                settings.Load(settingsXml);
                XmlElement r = settings.DocumentElement;
                menuBar.Visible = (r.ChildNodes[0].Attributes[0].Value.Equals("True"));
        
                linkBar.Visible = (r.ChildNodes[2].Attributes[0].Value.Equals("True"));
             
                homePage = r.ChildNodes[5].InnerText;
            }

             homePage = settings.DocumentElement.ChildNodes[5].InnerText;
        }
        private void themtab_Click(object sender, EventArgs e)
        {
             addNewTab();

            
            if (getCurrentBrowser().Url != null)
                    adrBarTextBox.Text = getCurrentBrowser().Url.ToString();
                else adrBarTextBox.Text = "about:blank";

                if (getCurrentBrowser().CanGoBack) toolStripButton1.Enabled = true;
                else toolStripButton1.Enabled = false;

                if (getCurrentBrowser().CanGoForward) toolStripButton2.Enabled = true;
                else toolStripButton2.Enabled = false;
            
        }


        //add favorite
        private void addFavorit(String url, string name)
        {
            XmlDocument myXml = new XmlDocument();
            XmlElement el = myXml.CreateElement("favorit");
            el.SetAttribute("url", url);
            el.InnerText = name;
            if (!File.Exists(favXml))
            {
                XmlElement root = myXml.CreateElement("favorites");
                myXml.AppendChild(root);
                root.AppendChild(el);
            }
            else
            {
                myXml.Load(favXml);
                myXml.DocumentElement.AppendChild(el);
            }
            if (panel1.Visible == true)
            {
                TreeNode node = new TreeNode(el.InnerText, faviconIndex(el.GetAttribute("url")), faviconIndex(el.GetAttribute("url")));
                node.ToolTipText = el.GetAttribute("url");
                node.Name = el.GetAttribute("url");
                node.ContextMenuStrip = favContextMenu;
                favTreeView.Nodes.Add(node);
            }
            myXml.Save(favXml);
        }
        private void addLink(String url, string name)
        {
            XmlDocument myXml = new XmlDocument();
            XmlElement el = myXml.CreateElement("link");
            el.SetAttribute("url", url);
            el.InnerText = name;

            if (!File.Exists(linksXml))
            {
                XmlElement root = myXml.CreateElement("links");
                myXml.AppendChild(root);
                root.AppendChild(el);
            }
            else
            {
                myXml.Load(linksXml);
                myXml.DocumentElement.AppendChild(el);
            }
            if (linkBar.Visible == true)
            {
                ToolStripButton b =
                          new ToolStripButton(el.InnerText, getFavicon(url), items_Click, el.GetAttribute("url"));
                b.ToolTipText = el.GetAttribute("url");
                b.MouseUp += new MouseEventHandler(b_MouseUp);
                linkBar.Items.Add(b);
            }

            if (panel1.Visible == true)
            {
                TreeNode node = new TreeNode(el.InnerText, faviconIndex(url), faviconIndex(el.GetAttribute("url")));
                node.Name = el.GetAttribute("url");
                node.ToolTipText = el.GetAttribute("url");
                node.ContextMenuStrip = linkContextMenu;
                favTreeView.Nodes[0].Nodes.Add(node);
            }
            myXml.Save(linksXml);
        }

        private void deleteLink()
        {
            if (panel1.Visible == true)
                favTreeView.Nodes[0].Nodes[adress].Remove();
            if (linkBar.Visible == true)
                linkBar.Items.RemoveByKey(adress);
            XmlDocument myXml = new XmlDocument();
            myXml.Load(linksXml);
            XmlElement root = myXml.DocumentElement;
            foreach (XmlElement x in root.ChildNodes)
            {
                if (x.GetAttribute("url").Equals(adress))
                {
                    root.RemoveChild(x);
                    break;
                }
            }

            myXml.Save(linksXml);
        }
        private void deleteFavorit()
        {
            favTreeView.SelectedNode.Remove();

            XmlDocument myXml = new XmlDocument();
            myXml.Load(favXml);
            XmlElement root = myXml.DocumentElement;
            foreach (XmlElement x in root.ChildNodes)
            {
                if (x.GetAttribute("url").Equals(adress))
                {
                    root.RemoveChild(x);
                    break;
                }
            }

            myXml.Save(favXml);

        }
        private void renameFavorit()
        {
            RenameLink rl = new RenameLink(name);
            if (rl.ShowDialog() == DialogResult.OK)
            {
                XmlDocument myXml = new XmlDocument();
                myXml.Load(favXml);
                foreach (XmlElement x in myXml.DocumentElement.ChildNodes)
                {
                    if (x.InnerText.Equals(name))
                    {
                        x.InnerText = rl.newName.Text;
                        break;
                    }
                }
                favTreeView.Nodes[adress].Text = rl.newName.Text;
                myXml.Save(favXml);
            }
            rl.Close();
        }

        private void renameLink()
        {
            RenameLink rl = new RenameLink(name);
            if (rl.ShowDialog() == DialogResult.OK)
            {
                XmlDocument myXml = new XmlDocument();
                myXml.Load(linksXml);
                foreach (XmlElement x in myXml.DocumentElement.ChildNodes)
                {
                    if (x.InnerText.Equals(name))
                    {
                        x.InnerText = rl.newName.Text;
                        break;
                    }
                }
                if (linkBar.Visible == true)
                    linkBar.Items[adress].Text = rl.newName.Text;
                if (panel1.Visible == true)
                    favTreeView.Nodes[0].Nodes[adress].Text = rl.newName.Text;
                myXml.Save(linksXml);
            }
            rl.Close();
        }
        //addHistory method
        private void addHistory(Uri url, string data)
        {
            XmlDocument myXml = new XmlDocument();
            int i = 1;
            XmlElement el = myXml.CreateElement("item");
            el.SetAttribute("url", url.ToString());
            el.SetAttribute("lastVisited", data);

            if (!File.Exists(historyXml))
            {
                XmlElement root = myXml.CreateElement("history");
                myXml.AppendChild(root);
                el.SetAttribute("times", "1");
                root.AppendChild(el);
            }
            else
            {
                myXml.Load(historyXml);

                foreach (XmlElement x in myXml.DocumentElement.ChildNodes)
                {
                    if (x.GetAttribute("url").Equals(url.ToString()))
                    {
                        i = int.Parse(x.GetAttribute("times")) + 1;
                        myXml.DocumentElement.RemoveChild(x);
                        break;
                    }
                }

                el.SetAttribute("times", i.ToString());
                myXml.DocumentElement.InsertBefore(el, myXml.DocumentElement.FirstChild);

                if (panel1.Visible == true)
                {
                
                    if (comboBox1.Text.Equals("Ordered Visited Today"))
                    {
                        if (!historyTreeView.Nodes.ContainsKey(url.ToString()))
                        {
                            TreeNode node =
                                 new TreeNode(url.ToString(), 3, 3);
                            node.ToolTipText = url.ToString() + "\nLast Visited: " + data + "\nTimes visited :" + i.ToString();
                            node.Name = url.ToString();
                            node.ContextMenuStrip = histContextMenu;
                            historyTreeView.Nodes.Insert(0, node);
                        }
                        else
                            historyTreeView.Nodes[url.ToString()].ToolTipText
                              = url.ToString() + "\nLast Visited: " + data + "\nTimes visited: " + i.ToString();
                    }
                
                    if (comboBox1.Text.Equals("View By Site"))
                    {
                        if (!historyTreeView.Nodes.ContainsKey(url.Host.ToString()))
                        {
                            historyTreeView.Nodes.Add(url.Host.ToString(), url.Host.ToString(), 0, 0);

                            TreeNode node =
                                   new TreeNode(url.ToString(), 3, 3);
                            node.ToolTipText = url.ToString() + "\nLast Visited: " + data + "\nTimes visited: " + i.ToString();
                            node.Name = url.ToString();
                            node.ContextMenuStrip = histContextMenu;
                            historyTreeView.Nodes[url.Host.ToString()].Nodes.Add(node);
                        }

                        else
                            if (!historyTreeView.Nodes[url.Host.ToString()].Nodes.ContainsKey(url.ToString()))
                        {
                            TreeNode node =
                                new TreeNode(url.ToString(), 3, 3);
                            node.ToolTipText = url.ToString() + "\nLast Visited: " + data + "\nTimes visited: " + i.ToString();
                            node.Name = url.ToString();
                            node.ContextMenuStrip = histContextMenu;
                            historyTreeView.Nodes[url.Host.ToString()].Nodes.Add(node);
                        }
                        else
                            historyTreeView.Nodes[url.Host.ToString()].Nodes[url.ToString()].ToolTipText
                                    = url.ToString() + "\nLast Visited: " + data + "\nTimes visited" + i.ToString();

                    }
                    /* view by date*/
                    //if (comboBox1.Text.Equals("View by Date"))
                    //{
                    //    if (historyTreeView.Nodes[4].Nodes.ContainsKey(url.ToString()))
                    //        historyTreeView.Nodes[url.ToString()].ToolTipText
                    //                = url.ToString() + "\nLast Visited: " + data + "\nTimes visited: " + i.ToString();
                    //    else
                    //    {
                    //        TreeNode node =
                    //            new TreeNode(url.ToString(), 3, 3);
                    //        node.ToolTipText = url.ToString() + "\nLast Visited: " + data + "\nTimes visited :" + i.ToString();
                    //        node.Name = url.ToString();
                    //        node.ContextMenuStrip = histContextMenu;
                    //        historyTreeView.Nodes[4].Nodes.Add(node);
                    //    }
                    //}
                }

            }
            myXml.Save(historyXml);
        }
        //delete history
        private void deleteHistory()
        {
            XmlDocument myXml = new XmlDocument();
            myXml.Load(historyXml);
            XmlElement root = myXml.DocumentElement;
            foreach (XmlElement x in root.ChildNodes)
            {
                if (x.GetAttribute("url").Equals(adress))
                {
                    root.RemoveChild(x);
                    break;
               }
            }
           historyTreeView.SelectedNode.Remove();
            myXml.Save(historyXml);
        }


// thêm tab mới 
        private void addNewTab()
        {
          
            TabPage tpage = new TabPage();
            tpage.BorderStyle = BorderStyle.Fixed3D;
            tabControl1.TabPages.Insert(tabControl1.TabCount - 1, tpage);
            WebBrowser browser = new WebBrowser();
            browser.ScriptErrorsSuppressed = true;
            browser.Navigate(homePage);
            tpage.Controls.Add(browser);
            browser.Dock = DockStyle.Fill;
            tabControl1.SelectTab(tpage);
            browser.ProgressChanged += new WebBrowserProgressChangedEventHandler(Form1_ProgressChanged);
            browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Form1_DocumentCompleted);
            browser.Navigating += new WebBrowserNavigatingEventHandler(Form1_Navigating);
            browser.CanGoBackChanged += new EventHandler(browser_CanGoBackChanged);
            browser.CanGoForwardChanged += new EventHandler(browser_CanGoForwardChanged);

        }


        private void Form1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            WebBrowser currentBrowser = getCurrentBrowser();
            this.toolStripStatusLabel1.Text = "Done";
            String text = "Blank Page";

            if (!currentBrowser.Url.ToString().Equals("about:blank"))
            {
                text = currentBrowser.Url.Host.ToString();
            }

            this.adrBarTextBox.Text = currentBrowser.Url.ToString();
            tabControl1.SelectedTab.Text = text;

            //img.Image = favicon(currentBrowser.Url.ToString(),"net.png" );

            if (!urls.Contains(currentBrowser.Url.Host.ToString()))
                urls.Add(currentBrowser.Url.Host.ToString());

            if (!currentBrowser.Url.ToString().Equals("about:blank") && currentBrowser.StatusText.Equals("Done"))
                addHistory(currentBrowser.Url, DateTime.Now.ToString(currentCulture));
        }

        private void Form1_ProgressChanged(object sender, WebBrowserProgressChangedEventArgs e)
        {
            //if (e.CurrentProgress < e.MaximumProgress)
            //    toolStripProgressBar1.Value = (int)e.CurrentProgress;
            //else toolStripProgressBar1.Value = toolStripProgressBar1.Maximum;

        }
        private void Form1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            this.toolStripStatusLabel1.Text = getCurrentBrowser().StatusText;

        }
 // đóng tab       
        private void closeTab()
        {

            if (tabControl1.TabCount != 2)
            {
                tabControl1.TabPages.RemoveAt(tabControl1.SelectedIndex);
            }


        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == tabControl1.TabPages.Count - 1) addNewTab();
            else
            {
                if (getCurrentBrowser().Url != null)
                    adrBarTextBox.Text = getCurrentBrowser().Url.ToString();
                else adrBarTextBox.Text = "about:blank";

                if (getCurrentBrowser().CanGoBack) toolStripButton1.Enabled = true;
                else toolStripButton1.Enabled = false;

                if (getCurrentBrowser().CanGoForward) toolStripButton2.Enabled = true;
                else toolStripButton2.Enabled = false;
            }
        }

       

        private void duplicateTabCtrlDToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            if (getCurrentBrowser().Url != null)
            {
                Uri dupurl = getCurrentBrowser().Url;
                addNewTab();
                getCurrentBrowser().Url = dupurl;

            }
            else addNewTab();
        }


        //favicon
        public static Image favicon(String u, string file)
        {

            Uri url = new Uri(u);
            String iconurl = "http://" + url.Host + "/favicon.ico";

            WebRequest request = WebRequest.Create(iconurl);
            try
            {
                WebResponse response = request.GetResponse();

                Stream s = response.GetResponseStream();
                return Image.FromStream(s);
            }
            catch (Exception ex)
            {
                return Image.FromFile(file);
            }


        }

        private int faviconIndex(string url)
        {
            Uri key = new Uri(url);
            if (!imgList.Images.ContainsKey(key.Host.ToString())) ;               
            return imgList.Images.IndexOfKey(key.Host.ToString());
        }

        private Image getFavicon(string key)
        {
            Uri url = new Uri(key);
            if (!imgList.Images.ContainsKey(url.Host.ToString())) ;              
            return imgList.Images[url.Host.ToString()];
        }

//address
        private WebBrowser getCurrentBrowser( )

        {
            webBrowser1.ScriptErrorsSuppressed = true;
            return (WebBrowser)tabControl1.SelectedTab.Controls[0]; 
            
        }

        private void adrBarTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                getCurrentBrowser().Navigate(adrBarTextBox.Text);

            }
        }
        //select all from adr bar
        
        private void adrBarTextBox_Click_1(object sender, EventArgs e)
        {
            adrBarTextBox.SelectAll();
        }
        //show urls

        private void showUrl()
        {
            if (File.Exists(historyXml))
            {
                XmlDocument myXml = new XmlDocument();
                myXml.Load(historyXml);
                //int i = 0;
                int num = int.Parse(settings.DocumentElement.ChildNodes[6].InnerText.ToString());
                foreach (XmlElement el in myXml.DocumentElement.ChildNodes)
                {
                    adrBarTextBox.Items.Add(el.GetAttribute("url").ToString());

                }
            }
        }

        private void adrBarTextBox_DropDown_1(object sender, EventArgs e)
        {
            adrBarTextBox.Items.Clear();
            showUrl();
        }

        private void adrBarTextBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            getCurrentBrowser().Navigate(adrBarTextBox.SelectedItem.ToString());
        }


        void browser_CanGoForwardChanged(object sender, EventArgs e)
        {
            toolStripButton2.Enabled = !toolStripButton2.Enabled;
        }
        //canGoBackChanged
        void browser_CanGoBackChanged(object sender, EventArgs e)
        {
            toolStripButton1.Enabled = !toolStripButton1.Enabled;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            getCurrentBrowser().GoBack();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            getCurrentBrowser().GoForward();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            getCurrentBrowser().Refresh();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            getCurrentBrowser().Stop();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            panel1.Visible = !panel1.Visible;
            settings.DocumentElement.ChildNodes[3].Attributes[0].Value = panel1.Visible.ToString();


        }
//add fv
        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            if (getCurrentBrowser().Url != null)
            {
                addfv dlg = new addfv(getCurrentBrowser().Url.ToString());
                DialogResult res = dlg.ShowDialog();

                if (res == DialogResult.OK)
                {
                    if (dlg.favFile == "Favorites")
                    {
                        addFavorit(getCurrentBrowser().Url.ToString(), dlg.favName);
                        dlg.Close();
                    }
                    else addLink(getCurrentBrowser().Url.ToString(), dlg.favName);

                    dlg.Close();
                }
            }
        }
//search
        private void toolStripTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                if (googleSearch.Checked == true)
                    getCurrentBrowser().Navigate("https://www.google.com/search?q=" + searchTextBox.Text);
                else
                    getCurrentBrowser().Navigate("http://search.live.com/results.aspx?q=" + searchTextBox.Text);
        }


        private void googleSearch_Click(object sender, EventArgs e)
        {
            liveSearch.Checked = !googleSearch.Checked;
        }

        private void liveSearch_Click(object sender, EventArgs e)
        {
            googleSearch.Checked = !liveSearch.Checked;
        }


        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            addLink(getCurrentBrowser().Url.ToString(), getCurrentBrowser().Url.ToString());
            
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            if (getCurrentBrowser().Url != null)
                addLink(getCurrentBrowser().Url.ToString(), getCurrentBrowser().Url.ToString());
        }


        //show link
        private void showLinks()
        {
            if (File.Exists(linksXml))
            {
                XmlDocument myXml = new XmlDocument();
                myXml.Load(linksXml);
                XmlElement root = myXml.DocumentElement;
                foreach (XmlElement el in root.ChildNodes)
                {
                    ToolStripButton b = new ToolStripButton(el.InnerText, getFavicon(el.GetAttribute("url")), items_Click, el.GetAttribute("url"));

                    b.ToolTipText = el.GetAttribute("url");
                    b.MouseUp += new MouseEventHandler(b_MouseUp);
                    linkBar.Items.Add(b);
                    
                }
            }
        }
       
        private void items_Click(object sender, EventArgs e)
        {
            ToolStripButton b = (ToolStripButton)sender;
            getCurrentBrowser().Navigate(b.ToolTipText);
          
        }
        private void b_MouseUp(object sender, MouseEventArgs e)
        {
            ToolStripButton b = (ToolStripButton)sender;
            adress = b.ToolTipText;
            name = b.Text;

            if (e.Button == MouseButtons.Right)
                linkContextMenu.Show(MousePosition);
        }

        private void linkBar_VisibleChanged(object sender, EventArgs e)
        {
            
            if (linkBar.Visible == true) showLinks();
            else while (linkBar.Items.Count > 12) linkBar.Items[linkBar.Items.Count - 1].Dispose();
        }

        //add to favorite
        private void addToFavoritesToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (getCurrentBrowser().Url != null)
            {
                addfv af = new addfv(getCurrentBrowser().Url.ToString());
                DialogResult res = af.ShowDialog();

                if (res == DialogResult.OK)
                {
                    if (af.favFile == "Favorites")
                    {
                        addFavorit(getCurrentBrowser().Url.ToString(), af.favName);
                        af.Close();
                    }
                    else addLink(getCurrentBrowser().Url.ToString(), af.favName);

                    af.Close();
                }
            }
        }


        private void addToFavoritesBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addLink(getCurrentBrowser().Url.ToString(), getCurrentBrowser().Url.ToString());
        }

        private void organizeFavoritesToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            (new OrganizeFavorites(favTreeView, linkBar, linkContextMenu, favContextMenu)).ShowDialog();
        }

        //show favorites in menu
        private void favoritesToolStripMenuItem_DropDownOpening_1(object sender, EventArgs e)
        {
            XmlDocument myXml = new XmlDocument();
            myXml.Load(favXml);

            for (int i = favoritesToolStripMenuItem.DropDownItems.Count - 1; i > 5; i--)
            {
                favoritesToolStripMenuItem.DropDownItems.RemoveAt(i);
            }
            foreach (XmlElement el in myXml.DocumentElement.ChildNodes)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(el.InnerText, getFavicon(el.GetAttribute("url")), fav_Click);
                item.ToolTipText = el.GetAttribute("url");
                favoritesToolStripMenuItem.DropDownItems.Add(item);
            }
        }
        //show links in menu

        private void linksToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            XmlDocument myXml = new XmlDocument();
            myXml.Load(linksXml);
            linksToolStripMenuItem.DropDownItems.Clear();
            foreach (XmlElement el in myXml.DocumentElement.ChildNodes)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(el.InnerText, getFavicon(el.GetAttribute("url")), fav_Click);
                item.ToolTipText = el.GetAttribute("url");
                linksToolStripMenuItem.DropDownItems.Add(item);
            }
        }

        private void fav_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem m = (ToolStripMenuItem)sender;
            getCurrentBrowser().Navigate(m.ToolTipText);
        }


        //FILE

        private void newTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addNewTab();
        }
        private void duplicateTabCtrlDToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (getCurrentBrowser().Url != null)
            {
                Uri dup_url = getCurrentBrowser().Url;
                addNewTab();
                getCurrentBrowser().Url = dup_url;

            }
            else addNewTab();
        }

        private void closeTabCtrlWToolStripMenuItem_Click(object sender, EventArgs e)
        {
            closeTab();
        }

        private void pageSetupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            getCurrentBrowser().ShowPageSetupDialog();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            getCurrentBrowser().ShowSaveAsDialog();
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            getCurrentBrowser().ShowPrintDialog();
        }

        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            getCurrentBrowser().ShowPrintPreviewDialog();
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            getCurrentBrowser().ShowPropertiesDialog();
        }

        //Edit

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            getCurrentBrowser().Document.ExecCommand("Cut", false, null);
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            getCurrentBrowser().Document.ExecCommand("Copy", false, null);
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            getCurrentBrowser().Document.ExecCommand("Paste", false, null);
        }


        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            getCurrentBrowser().Document.ExecCommand("SelectAll", true, null);
        }



        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            tabControl1.SelectedTab.Text = webBrowser1.Document.Title;
        }

        private void toolStripComboBox1_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            
                getCurrentBrowser().Navigate(adrBarTextBox.Text);

        }
        private void newWindowCtrlNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new TKBrowser()).Show();
        }

        

        private void toolStripMenuItem16_Click(object sender, EventArgs e)
        {
            closeTab();
        }

        private void toolStripMenuItem17_Click(object sender, EventArgs e)
        {
            if (getCurrentBrowser().Url != null)
            {
                Uri dup_url = getCurrentBrowser().Url;
                addNewTab();
                getCurrentBrowser().Url = dup_url;

            }
            else addNewTab();
        }

        private void openToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            getCurrentBrowser().Navigate(adress);
        }

        private void openInNewTabToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            addNewTab();
            getCurrentBrowser().Navigate(adress);
            
        }

        private void openInNewWindowToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TKBrowser new_form = new TKBrowser();
            new_form.Show();
            new_form.getCurrentBrowser().Navigate(adress);
        }

        private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            deleteLink();
        }

        private void renameToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            renameLink();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            getCurrentBrowser().Navigate(adress);
        }

        private void openInNewTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addNewTab();
            getCurrentBrowser().Navigate(adress);
        }

        private void openInNewWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TKBrowser new_form = new TKBrowser();
            new_form.Show();
            new_form.getCurrentBrowser().Navigate(adress);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            deleteFavorit();
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renameFavorit();
        }

        private void historyTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                historyTreeView.SelectedNode = e.Node;
                adress = e.Node.Text;
            }
            else
                if (!comboBox1.Text.Equals("Ordered Visited Today"))
            {
                if (!historyTreeView.Nodes.Contains(e.Node))
                    getCurrentBrowser().Navigate(e.Node.Text);
            }
            else
                getCurrentBrowser().Navigate(e.Node.Text);
        }

        private void panel1_VisibleChanged(object sender, EventArgs e)
        {
            if (panel1.Visible == true)
            {
                showFavorites();
                showHistory();
            }
            else
            {
                favTreeView.Nodes.Clear();
                historyTreeView.Nodes.Clear();
            }
        }
//show favorite ,history
        private void showFavorites()
        {
            XmlDocument myXml = new XmlDocument();
            TreeNode link = new TreeNode("Links", 0, 0);
            link.NodeFont = new Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            favTreeView.Nodes.Add(link);

            if (File.Exists(favXml))
            {
                myXml.Load(favXml);

                foreach (XmlElement el in myXml.DocumentElement.ChildNodes)
                {
                    TreeNode node =
                        new TreeNode(el.InnerText, faviconIndex(el.GetAttribute("url")), faviconIndex(el.GetAttribute("url")));
                    node.ToolTipText = el.GetAttribute("url");
                    node.Name = el.GetAttribute("url");
                    node.ContextMenuStrip = favContextMenu;
                    favTreeView.Nodes.Add(node);
                }

            }

            if (File.Exists(linksXml))
            {
                myXml.Load(linksXml);

                foreach (XmlElement el in myXml.DocumentElement.ChildNodes)
                {
                    TreeNode node =
                        new TreeNode(el.InnerText, faviconIndex(el.GetAttribute("url")), faviconIndex(el.GetAttribute("url")));
                    node.ToolTipText = el.GetAttribute("url");
                    node.Name = el.GetAttribute("url");
                    node.ContextMenuStrip = linkContextMenu;
                    favTreeView.Nodes[0].Nodes.Add(node);
                }

            }

        }

        private void favTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void favTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                favTreeView.SelectedNode = e.Node;
                adress = e.Node.ToolTipText;
                name = e.Node.Text;
            }
            else
               if (e.Node != favTreeView.Nodes[0])
                getCurrentBrowser().Navigate(e.Node.ToolTipText);
        }

        

        private void closeTabContext_Click(object sender, EventArgs e)
        {
            closeTab();
        }

        private void closeTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            closeTab();
        }

        private void duplicateTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (getCurrentBrowser().Url != null)
            {
                Uri dup_url = getCurrentBrowser().Url;
                addNewTab();
                getCurrentBrowser().Url = dup_url;

            }
            else addNewTab();
        }

     

        private void showHistory()
        {
            historyTreeView.Nodes.Clear();
            XmlDocument myXml = new XmlDocument();

            if (File.Exists(historyXml))
            {
                myXml.Load(historyXml);
                DateTime now = DateTime.Now;
                if (comboBox1.Text.Equals("Ordered Visited Today"))
                {
                    historyTreeView.ShowRootLines = false;
                    foreach (XmlElement el in myXml.DocumentElement.ChildNodes)
                    {
                        DateTime d = DateTime.Parse(el.GetAttribute("lastVisited"), currentCulture);

                        if (!(d.Date == now.Date)) return;

                        TreeNode node =
                            new TreeNode(el.GetAttribute("url"), 3, 3);
                        node.ToolTipText = el.GetAttribute("url") + "\nLast Visited: " + el.GetAttribute("lastVisited") + "\nTimes Visited: " + el.GetAttribute("times");
                        node.Name = el.GetAttribute("url");
                        node.ContextMenuStrip = histContextMenu;
                        historyTreeView.Nodes.Add(node);
                    }

                }

                if (comboBox1.Text.Equals("View By Site"))
                {
                    historyTreeView.ShowRootLines = true;
                    foreach (XmlElement el in myXml.DocumentElement.ChildNodes)
                    {
                        Uri site = new Uri(el.GetAttribute("url"));

                        if (!historyTreeView.Nodes.ContainsKey(site.Host.ToString()))
                            historyTreeView.Nodes.Add(site.Host.ToString(), site.Host.ToString(), 0, 0);
                        TreeNode node = new TreeNode(el.GetAttribute("url"), 3, 3);
                        node.ToolTipText = el.GetAttribute("url") + "\nLast Visited: " + el.GetAttribute("lastVisited") + "\nTimes Visited: " + el.GetAttribute("times");
                        node.Name = el.GetAttribute("url");
                        node.ContextMenuStrip = histContextMenu;
                        historyTreeView.Nodes[site.Host.ToString()].Nodes.Add(node);
                    }

                }

                if (comboBox1.Text.Equals("View by Date"))
                {
                    historyTreeView.ShowRootLines = true;
                    historyTreeView.Nodes.Add("2 Weeks Ago", "2 Weeks Ago", 2, 2);
                    historyTreeView.Nodes.Add("Last Week", "Last Week", 2, 2);
                    historyTreeView.Nodes.Add("This Week", "This Week", 2, 2);
                    historyTreeView.Nodes.Add("Yesterday", "Yesterday", 2, 2);
                    historyTreeView.Nodes.Add("Today", "Today", 2, 2);
                    foreach (XmlElement el in myXml.DocumentElement.ChildNodes)
                    {
                        DateTime d = DateTime.Parse(el.GetAttribute("lastVisited"), currentCulture);

                        TreeNode node = new TreeNode(el.GetAttribute("url"), 3, 3);
                        node.ToolTipText = el.GetAttribute("url") + "\nLast Visited: " + el.GetAttribute("lastVisited") + "\nTimes Visited: " + el.GetAttribute("times");
                        node.Name = el.GetAttribute("url");
                        node.ContextMenuStrip = histContextMenu;

                        if (d.Date == now.Date)
                            historyTreeView.Nodes[4].Nodes.Add(node);
                        else
                            if (d.AddDays(1).ToShortDateString().Equals(now.ToShortDateString()))
                            historyTreeView.Nodes[3].Nodes.Add(node);
                        else
                                if (d.AddDays(7) > now)
                            historyTreeView.Nodes[2].Nodes.Add(node);
                        else
                                    if (d.AddDays(14) > now)
                            historyTreeView.Nodes[1].Nodes.Add(node);
                        else
                                        if (d.AddDays(21) > now)
                            historyTreeView.Nodes[0].Nodes.Add(node);
                        else
                                            if (d.AddDays(22) > now)
                            myXml.DocumentElement.RemoveChild(el);
                    }
                }
            }


        }

        private void deleteBrowserHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            DeleteBrowsingHistory bd = new DeleteBrowsingHistory();
            if (bd.ShowDialog() == DialogResult.OK)
            {
                if (bd.History.Checked == true)
                {
                    File.Delete(historyXml);
                   historyTreeView.Nodes.Clear();
                }
                if (bd.TempFiles.Checked == true)
                {
                    urls.Clear();
                    while (imgList.Images.Count > 4)
                        imgList.Images.RemoveAt(imgList.Images.Count - 1);
                    File.Delete("source.txt");

                }
            }
        }

       

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InternetOption intOp = new InternetOption(getCurrentBrowser().Url.ToString());
            if (intOp.ShowDialog() == DialogResult.OK)
            {
                if (!intOp.homepage.Text.Equals(""))
                {
                    homePage = intOp.homepage.Text;
                    settings.DocumentElement.ChildNodes[5].InnerText = intOp.homepage.Text;
                }
                if (intOp.deleteHistory.Checked == true)
                {
                    File.Delete(historyXml);
                    historyTreeView.Nodes.Clear();
                }
                settings.DocumentElement.ChildNodes[6].InnerText = intOp.num.Value.ToString();
                ActiveForm.ForeColor = intOp.forecolor;
                ActiveForm.BackColor = intOp.backcolor;
                linkBar.BackColor = intOp.backcolor;
                ActiveForm.Font = intOp.font;
                linkBar.Font = intOp.font;
                menuBar.Font = intOp.font;
            }



        }
        //FAKE IP


        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            toolStripComboBox1.Visible = !toolStripComboBox1.Visible;
            toolStripButton11.Visible = !toolStripButton11.Visible;
            toolStripButton12.Visible = false;

            var http = new HttpRequest();
            string codeHtml = http.Get("https://www.proxynova.com/proxy-server-list/country-vn/").ToString();
            string[] mangIP = new string[99];
            string[] mangPort = new string[99];
            int dem = 0;

            while (codeHtml.IndexOf("');</script>") != -1)
            {

                string ip = codeHtml.Substring(codeHtml.IndexOf("<script>document.write('") 
                    + 24, codeHtml.IndexOf("');</script>") - codeHtml.IndexOf("<script>document.write('") - 24);
                mangIP[dem] = ip;

                string port = codeHtml.Substring(codeHtml.IndexOf("');</script>") + 121, 5).Trim();
                mangPort[dem] = port;
                dem = dem + 1;
                codeHtml = codeHtml.Substring(codeHtml.IndexOf("');</script>") + 12);
            }
            for (int i = 0; i < dem; i++)
            {
                toolStripComboBox1.Items.Add(mangIP[i] + ":" + mangPort[i]);
            }

            //var http = new HttpRequest();
            //http.AddHeader(HttpHeader.UserAgent, Http.ChromeUserAgent());
            //string codeHtml = http.Get("https://spys.one/en/socks-proxy-list/").ToString();
            //string[] mangIP = new string[30];
            //string[] mangTocDo = new string[30];
            //string htmlTam = codeHtml;
            //for (int i = 0; i < 30; i++)
            //{
            //    mangIP[i] = htmlTam.Substring(htmlTam.IndexOf("<td colspan=1><font class=spy14>") + 32, htmlTam.IndexOf(@"<script type=""text/javascript"">document.write(") - htmlTam.IndexOf("<td colspan=1><font class=spy14>") - 32);
            //    htmlTam = htmlTam.Remove(htmlTam.IndexOf("<td colspan=1><font class=spy14>"), htmlTam.IndexOf(@"<script type=""text/javascript"">document.write(") - htmlTam.IndexOf("<td colspan=1><font class=spy14>") + 48);
            //    // Console.WriteLine(mangIP[i]);
            //}
            //htmlTam = codeHtml;
            //for (int i = 0; i < 30; i++)
            //{
            //    mangTocDo[i] = htmlTam.Substring(htmlTam.IndexOf("/td><td colspan=1><TABLE width='") + 32, htmlTam.IndexOf("' height='8' CELLPADDING=0 CELLSPACING=0>") - htmlTam.IndexOf("/td><td colspan=1><TABLE width='") - 32);
            //    htmlTam = htmlTam.Remove(htmlTam.IndexOf("/td><td colspan=1><TABLE width='"), htmlTam.IndexOf("' height='8' CELLPADDING=0 CELLSPACING=0>") - htmlTam.IndexOf("/td><td colspan=1><TABLE width='") + 73);
            //    // Console.WriteLine(mangTocDo[i]);

            //}
            //htmlTam = codeHtml;
            //string portGiaiMa = htmlTam.Substring(htmlTam.IndexOf(@"<tr><td>&nbsp;</td></tr><tr><td>&nbsp;</td></tr></td></tr></table><script type=""text/javascript"">") + 97, htmlTam.IndexOf(";</script>") - htmlTam.IndexOf(@"<tr><td>&nbsp;</td></tr><tr><td>&nbsp;</td></tr></td></tr></table><script type=""text/javascript"">") - 97);
            //string[] codeMaHoa = portGiaiMa.Split(';');

            //string[] mangPortMaHoa = new string[10];

            //for (int i = 10; i < codeMaHoa.Length; i++)
            //{
            //    mangPortMaHoa[i - 10] = codeMaHoa[i].Remove(codeMaHoa[i].IndexOf("="), 2);
            //}
            //string[] mangPort = new string[30];
            //for (int i = 0; i < 30; i++)
            //{
            //    mangPort[i] = htmlTam.Substring(htmlTam.IndexOf(@"<script type=""text/javascript"">document.write(""<font class=spy2>:<\/font>""") + 76, htmlTam.IndexOf("</script></font></td><td colspan=1>") - htmlTam.IndexOf(@"<script type=""text/javascript"">document.write(""<font class=spy2>:<\/font>""") - 78);
            //    htmlTam = htmlTam.Remove(htmlTam.IndexOf(@"<script type=""text/javascript"">document.write(""<font class=spy2>:<\/font>"""), htmlTam.IndexOf("</script></font></td><td colspan=1>") - htmlTam.IndexOf(@"<script type=""text/javascript"">document.write(""<font class=spy2>:<\/font>""") + 76);




            //}
            //for (int i = 0; i < 30; i++)
            //{
            //    string[] mangNoiPort = mangPort[i].Split(')', '+', '(');
            //    string ketQuaNoi = "";
            //    for (int j = 0; j < mangNoiPort.Length; j++)
            //    {
            //        for (int z = 0; z < 10; z++)
            //        {
            //            if (mangNoiPort[j] == mangPortMaHoa[z])
            //            {
            //                ketQuaNoi = ketQuaNoi + z.ToString();
            //                break;
            //            }
            //        }


            //    }
            //    mangPort[i] = ketQuaNoi;

            //}
            //for (int i = 0; i <= 30; i++)
            //{

            //    for (int j = i + 1; j < 30; j++)
            //    {
            //        int soTruoc = Convert.ToInt32(mangTocDo[i]);
            //        int soSau = Convert.ToInt32(mangTocDo[j]);
            //        if (soTruoc < soSau)
            //        {
            //            string temp;
            //            temp = mangTocDo[i];
            //            mangTocDo[i] = mangTocDo[j];
            //            mangTocDo[j] = temp;
            //            temp = mangIP[i];
            //            mangIP[i] = mangIP[j];
            //            mangIP[j] = temp;
            //            temp = mangPort[i];
            //            mangPort[i] = mangPort[j];
            //            mangPort[j] = temp;
            //        }

            //    }
            //}
            //for (int i = 0; i < 30; i++)
            //{
            //    toolStripComboBox1.Items.Add(mangIP[i] + ":" + mangPort[i]);
            //}

        }
        



       

        private void toolStripMenuItem13_Click(object sender, EventArgs e)
        {

            deleteLink();

        }

        private void toolStripMenuItem14_Click(object sender, EventArgs e)
        {
            renameLink();
        }

        private void toolStripMenuItem10_Click(object sender, EventArgs e)
        {
            getCurrentBrowser().Navigate(adress);
        }

        private void toolStripMenuItem11_Click(object sender, EventArgs e)
        {
            addNewTab();
            getCurrentBrowser().Navigate(adress);
        }

        private void toolStripMenuItem12_Click(object sender, EventArgs e)
        {
            TKBrowser new_form = new TKBrowser();
            new_form.Show();
            new_form.getCurrentBrowser().Navigate(adress);
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            if (toolStripComboBox1.Text == "")
            {
               getCurrentBrowser().Navigate(adrBarTextBox.Text);
                
            }
            else
            {
                fakeip();
            }

        }

        private void scanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ocr o = new ocr();
            o.Visible = true;

        }

        private void img_Click(object sender, EventArgs e)
        {
            getCurrentBrowser().Navigate("https://www.google.com/webhp");
        }

        private void sourceToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            String source = ("source.txt");
            StreamWriter writer = File.CreateText(source);
            writer.Write(getCurrentBrowser().DocumentText);
            writer.Close();
            Process.Start("notepad.exe", source);
        }

        private void toolStripDropDownButton1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton9_Click_1(object sender, EventArgs e)
        {
            getCurrentBrowser().Navigate("https://www.google.com/webhp");
        }

        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            getCurrentBrowser().Navigate("https://accounts.google.com/signin/v2/identifier?service=mail&lp=1&flowName=GlifWebSignIn&flowEntry=ServiceLogin");
        }

        private void toolStripButton14_Click(object sender, EventArgs e)
        {
            getCurrentBrowser().Navigate("https://www.instagram.com/");
        }

        private void toolStripButton15_Click(object sender, EventArgs e)
        {
            getCurrentBrowser().Navigate("https://www.facebook.com/");
        }

       
        

        private void fakeip()
        {
            try
            {
                fakeipp();
                MessageBox.Show("ĐÃ THAY THẾ PROXY THÀNH CÔNG");
                toolStripButton12.Visible = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show("truy cap thất bại ");
            }
        }
        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            reg_key.SetValue("ProxyEnable", 0);
            MessageBox.Show("ĐÃ HỦY !");
            toolStripComboBox1.Text = "";
        }
        private void fakeipp()
        {
          

           
            try
            {

                reg_key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
                string proxy = toolStripComboBox1.Text;  
                reg_key.SetValue("ProxyEnable", 1);
                reg_key.SetValue("ProxyServer", proxy);

                getCurrentBrowser().Navigate("https://www.google.com/webhp");

            }
            catch
            {
                MessageBox.Show("loi");
            }
        }








    }
}

