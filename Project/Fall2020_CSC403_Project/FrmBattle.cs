﻿using Fall2020_CSC403_Project.code;
using Fall2020_CSC403_Project.Properties;
using MyGameLibrary;
using System;
using System.Drawing;
using System.Media;
using System.Reflection;
using System.Windows.Forms;

namespace Fall2020_CSC403_Project {
  public partial class FrmBattle : Form {
    public static FrmBattle instance = null;
    private Enemy enemy;
    private Player player;

    //this helps keep track of whether or not this is a boss battle and if the boss is defeated
    public BossDefeatedWrapper bossIsDefeatedReference;
    private bool isBossBattle = false;

    //just used to keep track of which level this is on
    private int level;

    //keeps track of whether or not player has used flee during an encounter
    bool fleeLocked = false;

    public SoundPlayer goose_death;
    public SoundPlayer oh_yeah;
    public SoundPlayer fail_sound;
    public SoundPlayer among_us_kill;
    public SoundPlayer jeff;

    private FrmBattle() {
      InitializeComponent();
      player = Game.player;
    }

    public void Setup() {
      // update for this enemy
      picEnemy.BackgroundImage = enemy.Img;
      picEnemy.Refresh();
      BackColor = enemy.Color;
      picBossBattle.Visible = false;
      if (player.WeaponEquiped != 0){
        weapon.Visible = true;
        if (player.WeaponEquiped == 1){
          weapon.Image = Resources.weapon1;
        }
        if (player.WeaponEquiped == 2){
          weapon.Image = Resources.rpg;
        }
      }
      if(this.level == 3) {
        picPlayer.BackgroundImage = global::Fall2020_CSC403_Project.Properties.Resources.player3;
      }

      // Observer pattern
      enemy.AttackEvent += PlayerDamage;
      player.AttackEvent += EnemyDamage;

      // show health and health packs
      UpdateHealthBars();
      HealthPackCountLabel.Text = player.HealthPackCount.ToString();

      PlayerLevel.Text = "Level " + GameState.player.level.ToString()
        + " Exp: " + GameState.player.experience.ToString()
        + "/" + GameState.player.experiencePerLevel.ToString();

      textBox1.BackColor = this.BackColor;
      textBox2.BackColor = this.BackColor;

      InitializeSounds();
    }

    public void SetupForBossBattle(int level) {
      PictureBox bossBattle = null;
      switch(level) {
        // kool-aid man
        case 1:
          bossBattle = picBossBattle;
          oh_yeah.Play();
          break;
        case 2:
          // rough rodents
          bossBattle = picBossBattleSquirrels;
          break;
        case 3:
          // beeg amongus
          bossBattle = picBossBattleImposter;
          jeff.Play();
          break;
      }

      bossBattle.Location = Point.Empty;
      bossBattle.Size = ClientSize;
      bossBattle.Visible = true;
      bossBattle.BringToFront();

      tmrFinalBattle.Enabled = true;

      isBossBattle = true;
    }

    public static FrmBattle GetInstance(Enemy enemy, int level) {
      instance = new FrmBattle();
      instance.enemy = enemy;
      instance.level = level;
      instance.Setup();
      return instance;
    }

    private void UpdateHealthBars() {
      float playerHealthPer = player.Health / (float)player.MaxHealth;
      float enemyHealthPer = enemy.Health / (float)enemy.MaxHealth;

      const int MAX_HEALTHBAR_WIDTH = 226;
      lblPlayerHealthFull.Width = (int)(MAX_HEALTHBAR_WIDTH * playerHealthPer);
      lblEnemyHealthFull.Width = (int)(MAX_HEALTHBAR_WIDTH * enemyHealthPer);

      lblPlayerHealthFull.Text = player.Health.ToString();
      lblEnemyHealthFull.Text = enemy.Health.ToString();
    }

    private void btnAttack_Click(object sender, EventArgs e) {

      player.OnAttack(-4);

      if (enemy.Health > 0) {
        enemy.OnAttack(-2);
      }
      
      /*if (player.WeaponEquiped){
        gunfireBlast.Visible = true;
        wait(50);
        gunfireBlast.Visible = false;
        wait(50);
        gunfireBlast.Visible = true;
        wait(100);
        gunfireBlast.Visible = false;
        wait(100);
        gunfireBlast.Visible = true;
        wait(100);
        gunfireBlast.Visible = false;
      }*/

      UpdateHealthBars();
      if (player.Health <= 0 || enemy.Health <= 0) {
        
        //Didn't rearrange this code much, this was just a convenient
        //way to add this experience gain;
        //if the player dies, then the experience gain doesn't matter
        if (enemy.Health <= 0)
        {
            player.EarnExperience(enemy.experience);
        }

        instance = null;
        

        //added this check to change the value in FrmLevel to true
        if (isBossBattle) {
          bossIsDefeatedReference.bossIsDefeated = true;
          //added this to display the win screen for this level
          //FrmWinLevel win_instance = FrmWinLevel.GetInstance();
          if (player.Health > 0)
          {
            Form win_instance;
            switch (this.level)
            {
              case 1:
                win_instance = new FrmWinLevel();
                break;
              case 2:
                win_instance = new FrmWinLevelTwo();
                break;
              case 3:
                win_instance = new FrmWinLevelThree();
                break;
              default:
                win_instance = new FrmWinLevel();
                break;
            }
            win_instance.Show();
          }
        }
        if (enemy.Health <= 0)
        {
          if (BackColor == Color.LightGray)
          {
            goose_death.Play();
          }
          if (BackColor == Color.White || BackColor == Color.LightBlue)
          {
            among_us_kill.Play();
          }
          enemy.AlterIsAlive(false);
        }
        if (player.Health <= 0)
        {
          player.AlterIsAlive(false);
        }
        if (!player.IsAlive)
        {
          FrmPlayerDeath playerDeathInstance = new FrmPlayerDeath();
          playerDeathInstance.Show();
        }
        Close();
      }
    }

    private void EnemyDamage(int amount) {
      enemy.AlterHealth(amount);
    }

    private void PlayerDamage(int amount) {
      player.AlterHealth(amount);
    }

    private void tmrFinalBattle_Tick(object sender, EventArgs e) {
      picBossBattle.Visible = false;
      picBossBattleSquirrels.Visible = false;
      picBossBattleImposter.Visible = false;
      tmrFinalBattle.Enabled = false;
    }

     private void Heal_Click(object sender, EventArgs e){
         if (player.HealthPackCount > 0 && player.Health != player.MaxHealth) {
            player.UseHealthPack();
            int healingPower = player.MaxHealth / 2;
            if (player.Health + healingPower > player.MaxHealth) {
                player.AlterHealth(player.MaxHealth - player.Health);
            }
            else {
                player.AlterHealth(healingPower);
            }
            UpdateHealthBars();
            HealthPackCountLabel.Text = player.HealthPackCount.ToString();
        }
     }

    // When clicked, player has a 1/2 chance of fleeing battle.
    // If failed, flee button does nothing
    private void FleeButton_Click(object sender, EventArgs e)
    {
      if (fleeLocked)
      {
        return;
      }
      fleeLocked = true;
      FleeButton.FlatStyle = FlatStyle.Flat;
      Random random = new Random();
      int chance = random.Next(1, 3);
      //System.Diagnostics.Debug.WriteLine(chance.ToString());
      if (chance == 1)
      {
        Close();
      }
      else
      {
        fail_sound.Play();
      }
    }

    // Found this code at: https://stackoverflow.com/questions/10458118/wait-one-second-in-running-program
    public void wait(int milliseconds)
    {
      var timer1 = new System.Windows.Forms.Timer();
      if (milliseconds == 0 || milliseconds < 0) return;

      // Console.WriteLine("start wait timer");
      timer1.Interval = milliseconds;
      timer1.Enabled = true;
      timer1.Start();

      timer1.Tick += (s, e) =>
      {
        timer1.Enabled = false;
        timer1.Stop();
        // Console.WriteLine("stop wait timer");
      };

      while (timer1.Enabled)
      {
        Application.DoEvents();
      }
    }

    private void PlayerLevel_Click(object sender, EventArgs e)
    {
      
    }

    private void textBox1_TextChanged(object sender, EventArgs e)
    {

    }

    private void InitializeSounds()
    {
      goose_death = new SoundPlayer(Resources.goose_death);
      goose_death.Load();
      oh_yeah = new SoundPlayer(Resources.oh_yeah);
      oh_yeah.Load();
      fail_sound = new SoundPlayer(Resources.fail_sound);
      fail_sound.Load();
      among_us_kill = new SoundPlayer(Resources.among_us_kill);
      among_us_kill.Load();
      jeff = new SoundPlayer(Resources.jeff);
      jeff.Load();
    }
  }
}
