using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Game;

namespace DamkaForm
{
    public class FormLogin : Form
    {
        private GameBoard.eGameboardSizes m_BoardSize;
        private TextBox TextboxUserName1 = new TextBox();
        private TextBox TextboxUserName2 = new TextBox();
        private CheckBox CheckBoxPlayer2 = new CheckBox();
        private Label LabelBoardSize = new Label();
        private RadioButton RadioButtonSix = new RadioButton();
        private RadioButton RadioButtonEight = new RadioButton();
        private RadioButton RadioButtonTen = new RadioButton();
        private Label LabelPlayers = new Label();
        private Label LabelUserName1 = new Label();
        private Label LabelUserName2 = new Label();
        private Button ButtonDone = new Button();
        private bool m_Is2PlayersMode = !true;

        public FormLogin()
        {
            this.Size = new Size(275, 300);
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "GameSetting";
        }

        /// <summary>
        /// This method will be called once, just before the first time the form is displayed
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            InitControls();
        }

        private void InitControls()
        {
            LabelBoardSize.Text = "Board Size:";
            LabelBoardSize.Location = new Point(15, 40);
            RadioButtonSix.Text = "6x 6";
            RadioButtonSix.Location = new Point(15, 60);
            RadioButtonSix.Size = new Size(50, 30);
            RadioButtonSix.Checked = true;
            this.RadioButtonSix.Click += new EventHandler(this.m_RadioButtonSix_Click);
            RadioButtonEight.Text = "8x 8";
            RadioButtonEight.Location = new Point(80, 60);
            RadioButtonEight.Size = new Size(50, 30);
            this.RadioButtonEight.Click += new EventHandler(this.m_RadioButtonEight_Click);
            RadioButtonTen.Text = "10x 10";
            RadioButtonTen.Location = new Point(145, 60);
            RadioButtonTen.Size = new Size(65, 30);
            this.RadioButtonTen.Click += new EventHandler(this.m_RadioButtonTen_Click);
            LabelPlayers.Text = "Players:";
            LabelPlayers.Size = new Size(45, 15);
            LabelPlayers.Location = new Point(15, 100);
            TextboxUserName1.Location = new Point(95, 120);
            LabelUserName1.Text = "Player 1:";
            LabelUserName1.Location = new Point(15, 120);
            LabelUserName1.Size = new Size(50, 15);
            TextboxUserName1.Text = "Player1";
           CheckBoxPlayer2.Location = new Point(15, 160);
            CheckBoxPlayer2.Size = new Size(15, 15);
            LabelUserName2.Text = "Player 2:";
            LabelUserName2.Location = new Point(35, 160);
            LabelUserName2.Size = new Size(50, 15);
            this.CheckBoxPlayer2.Click += new EventHandler(this.m_CheckBoxPlayer2_Click);
            TextboxUserName2.Text = "Computer";
            TextboxUserName2.Enabled = false;
            TextboxUserName2.Location = new Point(95, 160);
            ButtonDone.Text = "Done";
            ButtonDone.Location = new Point(120, 200);
            this.ButtonDone.Click += new EventHandler(this.m_ButtonDone_Click);
            this.Controls.AddRange(new Control[]
            {
                LabelBoardSize,
                RadioButtonSix,
                RadioButtonEight,
                RadioButtonTen,
                LabelPlayers,
                LabelUserName1,
                TextboxUserName1,
                CheckBoxPlayer2,
                LabelUserName2,
                TextboxUserName2,
                ButtonDone
            });
        }

        private void m_CheckBoxPlayer2_Click(object sender, EventArgs e)
        {
            TextboxUserName2.Enabled = !TextboxUserName2.Enabled;
            m_Is2PlayersMode = TextboxUserName2.Enabled;

            if (m_Is2PlayersMode)
            {
                TextboxUserName2.Text = "Player2";
            }
            else
            {
                TextboxUserName2.Text = "Computer";
            }
        }

        private void m_RadioButtonSix_Click(object sender, EventArgs e)
        {
            m_BoardSize = GameBoard.eGameboardSizes.Small;
        }

        private void m_RadioButtonEight_Click(object sender, EventArgs e)
        {
            RadioButtonSix.Checked = !true;
            m_BoardSize = GameBoard.eGameboardSizes.Medium;
        }

        private void m_RadioButtonTen_Click(object sender, EventArgs e)
        {
            RadioButtonSix.Checked = !true;
            m_BoardSize = GameBoard.eGameboardSizes.Large;
        }

        public bool Is2PlayersMode()
        {
            return m_Is2PlayersMode;
        }

        public string UserName1
        {
            get { return TextboxUserName1.Text; }
        }

        public string UserName2
        {
            get { return TextboxUserName2.Text; }
        }

        public GameBoard.eGameboardSizes GameBoardSize
        {
            get { return m_BoardSize; }
        }

        private void m_ButtonDone_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}