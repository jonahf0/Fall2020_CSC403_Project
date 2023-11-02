﻿using Fall2020_CSC403_Project.code;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fall2020_CSC403_Project
{
  public partial class FrmStartScreen : Form
  {
    public FrmStartScreen()
    {
      InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e)
    {
      GameState.startGame = true;
      this.Close();
    }

    private void button2_Click(object sender, EventArgs e)
    {
      Application.Exit();
    }

    public void setContinueButtonText(string text)
    {
      this.button1.Text = text;
    }

    //easy way to create a new pause screen
    public static void displayStartScreen()
    {
      FrmStartScreen pauseScreen = new FrmStartScreen();
      pauseScreen.setContinueButtonText("Continue");
      GameState.isGamePaused = true;
      DateTime pauseStartTime = DateTime.Now;
      pauseScreen.ShowDialog();

      //this generates a corrective factor to simulate pausing the timer in the FrmLevel;
      //this makes the time pause
      GameState.totalPausedTime = DateTime.Now - pauseStartTime;
      GameState.isGamePaused = false;
    }

    private void Controls_Click(object sender, EventArgs e)
    {
      this.label2.Visible = !this.label2.Visible;
    }

    private void button3_Click(object sender, EventArgs e)
    {
      this.label1.Visible = !this.label1.Visible;
    }
  }
}
