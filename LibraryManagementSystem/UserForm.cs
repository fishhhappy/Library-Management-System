﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using System.Data.SQLite;

namespace LibraryManagementSystem
{
    public partial class UserForm : MaterialForm
    {
        private readonly MaterialSkinManager materialSkinManager;
        private string user_id;
        private AdminForm adminForm = null;
        int pagenum = 0;

        List<string[]> listG;

        public void setAdminForm(AdminForm adminForm)
        {
            this.adminForm = adminForm;
            adminModeButton.Enabled = true;
        }

        public void set_user_id(string user_id)
        {
            this.user_id = user_id;
            seedListView();
        }

        public UserForm()
        {
            InitializeComponent();

            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            dayModeButton.Enabled = false;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.LightBlue700, Primary.LightBlue900, Primary.LightBlue500, Accent.LightBlue200, TextShade.WHITE);
            blueSchemeButton.Enabled = false;

            listG = new List<string[]>();
        }

        private void seedListView()
        {
            //借阅信息表
            SQLiteCommand comm = new SQLiteCommand("select * from BorrowRecord, BookEPC, Book where BorrowRecord.user_id = '" + user_id + "'and BorrowRecord.book_epc = BookEPC.book_epc and BookEPC.book_id = Book.book_id;", LoginForm.conn);
            SQLiteDataReader read = comm.ExecuteReader();
            List<string[]> list = new List<string[]>();
            int num = 1;
            while (read.Read())
            {
                this.borrowmentList.BeginUpdate();
                if (read["return_time"].ToString() == "")
                {
                    string[] ss = { num.ToString(),read["book_name"].ToString(),
                                      read["borrow_time"].ToString(), read["due_time"].ToString(),"No"};
                    list.Add(ss);
                }
                else
                {
                    string[] ss = { num.ToString(),read["book_name"].ToString(),
                                      read["borrow_time"].ToString(), read["due_time"].ToString(),"Yes"};
                    list.Add(ss);
                }
                num++;
            }
            comm.Dispose();
            read.Dispose();

            foreach (string[] ss in list)
            {
                ListViewItem item = new ListViewItem(ss);
                borrowmentList.Items.Add(item);
            }
            this.borrowmentList.EndUpdate();

            //我的资料表
            comm = new SQLiteCommand("select * from User where user_id = '" + user_id + "';",
                   LoginForm.conn);
            read = comm.ExecuteReader();

            if (read.Read())
            {
                userIDText.Text = read["user_id"].ToString();
                userNameText.Text = read["user_name"].ToString();
                userGenderText.Text = read["user_gender"].ToString() == "0" ? "女" : "男";
                userTypeText.Text = read["user_type"].ToString();
            }
            comm.Dispose();
            read.Dispose();

        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            pagenum = 1;
            lastPageButton.Visible = true;
            nextPageButton.Visible = true;
            showPageInfoText.Visible = true;

            bookInfo1.Clear();
            bookInfo2.Clear();
            bookInfo3.Clear();

            string keyword = searchText.Text;
            listG.Clear();
            if (searchCombo.Text == "作者")
            {
                SQLiteCommand comm = new SQLiteCommand("select * from Book where book_author = '"
                    + searchText.Text + "';", LoginForm.conn);
                SQLiteDataReader read = comm.ExecuteReader();
                while (read.Read())
                {
                    string[] ss = { read["book_name"].ToString(), read["book_author"].ToString(),
                                      read["book_type"].ToString(), read["book_pub"].ToString(),
                                      read["book_pubdate"].ToString()};
                    listG.Add(ss);
                }
                comm.Dispose();
                read.Dispose();

            }
            else if (searchCombo.Text == "ISBN")
            {
                SQLiteCommand comm = new SQLiteCommand("select * from Book where book_id = '"
                    + searchText.Text + "';", LoginForm.conn);
                SQLiteDataReader read = comm.ExecuteReader();
                while (read.Read())
                {
                    string[] ss = { read["book_name"].ToString(), read["book_author"].ToString(),
                                      read["book_type"].ToString(), read["book_pub"].ToString(),
                                      read["book_pubdate"].ToString() };
                    listG.Add(ss);
                }
                comm.Dispose();
                read.Dispose();
            }
            else//其他默认都按题名来查询
            {
                SQLiteCommand comm = new SQLiteCommand("select * from Book where book_name = '" + searchText.Text + "';", LoginForm.conn);
                SQLiteDataReader read = comm.ExecuteReader();
                while (read.Read())
                {
                    string[] ss = { read["book_name"].ToString(), read["book_author"].ToString(),
                                      read["book_type"].ToString(), read["book_pub"].ToString(),
                                      read["book_pubdate"].ToString() };
                    listG.Add(ss);
                }
                comm.Dispose();
                read.Dispose();

            }
            int num = 1;
            foreach (string[] ss in listG)
            {
                if (num >= 1 && num <= 3)
                {
                    if (num == 1)
                    {
                        bookInfo1.Text = "书名： " + ss[0] + " 作者： " + ss[1] + " 书籍类型： "
                            + ss[2] + " 出版社： " + ss[3] + " 出版时间： " + ss[4];
                    }
                    else if (num == 2)
                    {
                        bookInfo2.Text = "书名： " + ss[0] + " 作者： " + ss[1] + " 书籍类型： "
                            + ss[2] + " 出版社： " + ss[3] + " 出版时间： " + ss[4];
                    }
                    else//num==3
                    {
                        bookInfo3.Text = "书名： " + ss[0] + " 作者： " + ss[1] + " 书籍类型： "
                            + ss[2] + " 出版社： " + ss[3] + " 出版时间： " + ss[4];
                    }

                }
                else
                    break;
                num++;
            }

        }

        private void UserForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void materialRaisedButton2_Click(object sender, EventArgs e)
        {
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            dayModeButton.Enabled = false;
            nightModeButton.Enabled = true;
        }

        private void materialRaisedButton3_Click(object sender, EventArgs e)
        {
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            dayModeButton.Enabled = true;
            nightModeButton.Enabled = false;
        }

        private void materialRaisedButton4_Click(object sender, EventArgs e)
        {
            materialSkinManager.ColorScheme = new ColorScheme(Primary.LightBlue700, Primary.LightBlue900, Primary.LightBlue500, Accent.LightBlue200, TextShade.WHITE);
            blueSchemeButton.Enabled = false;
            greenSchemeButton.Enabled = true;
            bluegreySchemeButton.Enabled = true;
        }

        private void materialRaisedButton5_Click(object sender, EventArgs e)
        {
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Green600, Primary.Green700, Primary.Green200, Accent.Red100, TextShade.WHITE);
            blueSchemeButton.Enabled = true;
            greenSchemeButton.Enabled = false;
            bluegreySchemeButton.Enabled = true;
        }

        private void materialRaisedButton6_Click(object sender, EventArgs e)
        {
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
            blueSchemeButton.Enabled = true;
            greenSchemeButton.Enabled = true;
            bluegreySchemeButton.Enabled = false;
        }

        private void nextPageButton_Click(object sender, EventArgs e)
        {
            if (pagenum < 5)
                pagenum++;
            showPageInfoText.Text = pagenum + "/5";
            bookInfo1.Clear();
            bookInfo2.Clear();
            bookInfo3.Clear();
            int num = 1;
            foreach (string[] ss in listG)
            {
                if (num >= (pagenum - 1) * 3 + 1 && num <= (pagenum - 1) * 3 + 3)
                {
                    if (num == (pagenum - 1) * 3 + 1)
                    {
                        bookInfo1.Text = "书名： " + ss[0] + " 作者： " + ss[1] + " 书籍类型： "
                            + ss[2] + " 出版社： " + ss[3] + " 出版时间： " + ss[4];
                    }
                    else if (num == (pagenum - 1) * 3 + 2)
                    {
                        bookInfo2.Text = "书名： " + ss[0] + " 作者： " + ss[1] + " 书籍类型： "
                            + ss[2] + " 出版社： " + ss[3] + " 出版时间： " + ss[4];
                    }
                    else//num==3
                    {
                        bookInfo3.Text = "书名： " + ss[0] + " 作者： " + ss[1] + " 书籍类型： "
                            + ss[2] + " 出版社： " + ss[3] + " 出版时间： " + ss[4];
                    }

                }
                num++;
            }

        }

        private void lastPageButton_Click(object sender, EventArgs e)
        {
            if (pagenum > 1)
                pagenum--;
            showPageInfoText.Text = pagenum + "/5";
            int num = 1;
            bookInfo1.Clear();
            bookInfo2.Clear();
            bookInfo3.Clear();
            foreach (string[] ss in listG)
            {
                if (num >= (pagenum - 1) * 3 + 1 && num <= (pagenum - 1) * 3 + 3)
                {
                    if (num == (pagenum - 1) * 3 + 1)
                    {
                        bookInfo1.Text = "书名： " + ss[0] + " 作者： " + ss[1] + " 书籍类型： "
                            + ss[2] + " 出版社： " + ss[3] + " 出版时间： " + ss[4];
                    }
                    else if (num == (pagenum - 1) * 3 + 2)
                    {
                        bookInfo2.Text = "书名： " + ss[0] + " 作者： " + ss[1] + " 书籍类型： "
                            + ss[2] + " 出版社： " + ss[3] + " 出版时间： " + ss[4];
                    }
                    else//num==3
                    {
                        bookInfo3.Text = "书名： " + ss[0] + " 作者： " + ss[1] + " 书籍类型： "
                            + ss[2] + " 出版社： " + ss[3] + " 出版时间： " + ss[4];
                    }

                }
                num++;
            }
        }

        private void materialRaisedButton12_Click(object sender, EventArgs e)
        {
            if (userOldPwText.Text != "" && userPwText.Text != "" && userValidateNewPwText.Text != "")
            {
                SQLiteCommand comm = new SQLiteCommand("select user_pwd from User where user_id = '"
                    + user_id + "' and user_pwd= '" + userOldPwText.Text + "'; ", LoginForm.conn);
                SQLiteDataReader read = comm.ExecuteReader();
                if (read.Read())
                {
                    if (userPwText.Text == userValidateNewPwText.Text)
                    {
                        comm = new SQLiteCommand("update User set user_pwd = '" + userPwText.Text
                            + "' where user_id = '" + user_id + "'", LoginForm.conn);
                        comm.ExecuteNonQuery();
                        MessageBox.Show("密码修改成功", "提示", MessageBoxButtons.OK,
                            MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    }
                    else
                    {
                        MessageBox.Show("新密码两次输入不一致，无法更新哦~", "提示", MessageBoxButtons.OK,
                            MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    }

                }
                else
                {
                    MessageBox.Show("旧密码输入不正确，无法更新哦~", "提示", MessageBoxButtons.OK,
                        MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                }

                comm.Dispose();
                read.Dispose();
            }
            else
            {
                MessageBox.Show("密码更改步骤不正确，无法更新哦~", "提示", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            }
            userOldPwText.Clear();
            userPwText.Clear();
            userValidateNewPwText.Clear();

        }

        private void adminModeButton_Click(object sender, EventArgs e)
        {
            Hide();
            adminForm.Show();
        }
    }
}
