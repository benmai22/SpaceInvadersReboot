using System;
using Godot;

public class MainMenu : HBoxContainer {

	[Export]
	public int extraLivesCost = 2000;
	[Export]

	public int doubleHealthCost = 25000;
	[Export]

	public int burstBulletCost = 50000;
	private RichTextLabel CurrencyText;
	private Button extraLivesBtn;
	private Button doubleHealthBtn;
	private Button burstBulletBtn;
	private int currentScore = 0;
	private int currentLives = 0;
	private int currentHealth = 0;
	private int burstBulletMode = 0;
	public override void _Ready () {
		CurrencyText = (RichTextLabel) GetNode ("VBoxStoreContainer/Panel/CurrencyText");
		extraLivesBtn = (Button) GetNode ("VBoxStoreContainer/HBoxContainer/VBoxContainer/ExtraLifeBtn");
		doubleHealthBtn = (Button) GetNode ("VBoxStoreContainer/HBoxContainer/VBoxContainer2/DoubleHealthBtn");
		burstBulletBtn = (Button) GetNode ("VBoxStoreContainer/HBoxContainer/VBoxContainer3/MissileBtn");
		extraLivesBtn.Text = extraLivesCost + " Points";
		doubleHealthBtn.Text = doubleHealthCost + " Points";
		burstBulletBtn.Text = burstBulletCost + " Points";
		LoadGame ();
		LoadCurrentLives ();
		LoadCurrentHealth ();
		LoadBurstBullet ();
	}

	public void LoadGame () {
		var saveGame = new File ();
		if (!saveGame.FileExists ("user://score.save")) {
			GD.Print ("No save file found! ");
			return; // Error!  We don't have a save to load.
		}
		// Load the file line by line and process that dictionary to restore the object
		// it represents.
		saveGame.Open ("user://score.save", (File.ModeFlags) (int) File.ModeFlags.Read);

		while (saveGame.GetPosition () < saveGame.GetLen ()) {
			// Get the saved dictionary from the next line in the save file            
			currentScore = Int32.Parse (saveGame.GetLine ());
			CurrencyText.BbcodeText = "[right]Currency : " + currentScore + "[/right]";

		}
		saveGame.Close ();
	}

	private void LoadCurrentLives () {
		var saveGame = new File ();
		if (!saveGame.FileExists ("user://lives.save")) {
			GD.Print ("No save file found! ");
			return; // Error!  We don't have a save to load.
		}
		// Load the file line by line and process that dictionary to restore the object
		// it represents.
		saveGame.Open ("user://lives.save", (File.ModeFlags) (int) File.ModeFlags.Read);

		while (saveGame.GetPosition () < saveGame.GetLen ()) {
			// Get the saved dictionary from the next line in the save file
			// var nodeData = new Godot.Collections.Dictionary<string, object>((Godot.Collections.Dictionary)JSON.Parse(saveGame.GetLine()).Result);
			currentLives = currentLives + (Int16.Parse (saveGame.GetLine ()));

		}
		saveGame.Close ();
	}

	private void LoadCurrentHealth () {
		var saveGame = new File ();
		if (!saveGame.FileExists ("user://health.save")) {
			GD.Print ("No save file found! ");
			return; // Error!  We don't have a save to load.
		}
		// Load the file line by line and process that dictionary to restore the object
		// it represents.
		saveGame.Open ("user://health.save", (File.ModeFlags) (int) File.ModeFlags.Read);

		while (saveGame.GetPosition () < saveGame.GetLen ()) {
			// Get the saved dictionary from the next line in the save file
			// var nodeData = new Godot.Collections.Dictionary<string, object>((Godot.Collections.Dictionary)JSON.Parse(saveGame.GetLine()).Result);
			currentHealth = (Int16.Parse (saveGame.GetLine ()));
			currentHealth = Mathf.Max (currentHealth, 100);
		}
		saveGame.Close ();
	}

	private void LoadBurstBullet () {
		var saveGame = new File ();
		if (!saveGame.FileExists ("user://burst.save")) {
			GD.Print ("No save file found! ");
			return; // Error!  We don't have a save to load.
		}
		// Load the file line by line and process that dictionary to restore the object
		// it represents.
		saveGame.Open ("user://burst.save", (File.ModeFlags) (int) File.ModeFlags.Read);

		while (saveGame.GetPosition () < saveGame.GetLen ()) {
			// Get the saved dictionary from the next line in the save file
			// var nodeData = new Godot.Collections.Dictionary<string, object>((Godot.Collections.Dictionary)JSON.Parse(saveGame.GetLine()).Result);
			burstBulletMode = (Int16.Parse (saveGame.GetLine ()));

		}
		saveGame.Close ();
	}

	public void OnExtraLivesClick () {
		if (currentScore < extraLivesCost) {
			FlashButton (extraLivesBtn, new Color (1, 0, 0));
		} else {
			currentLives += 1;
			currentScore -= extraLivesCost;
			SaveGame ();
			CurrencyText.BbcodeText = "[right]Currency : " + currentScore + "[/right]";
			FlashButton (extraLivesBtn, new Color (0, 1, 0));
		}
	}

	public void OnDoubleHealthClick () {
		if (currentScore < doubleHealthCost) {
			FlashButton (doubleHealthBtn, new Color (1, 0, 0));
		} else {
			currentHealth *= 2;
			currentScore -= doubleHealthCost;
			SaveGame ();
			CurrencyText.BbcodeText = "[right]Currency : " + currentScore + "[/right]";
			FlashButton (doubleHealthBtn, new Color (0, 1, 0));
		}
	}

	public void OnBurstBulletClick () {
		if (currentScore < burstBulletCost) {
			FlashButton (burstBulletBtn, new Color (1, 0, 0));
		} else {
			burstBulletMode = 1;
			currentScore -= burstBulletCost;
			SaveGame ();
			CurrencyText.BbcodeText = "[right]Currency : " + currentScore + "[/right]";
			FlashButton (burstBulletBtn, new Color (0, 1, 0));
		}
	}

	async private void FlashButton (Button btn, Color color) {
		btn.SelfModulate = color;
		await ToSignal (GetTree ().CreateTimer (2), "timeout");
		btn.SelfModulate = new Color (1, 1, 1);
	}

	public void SaveGame () {
		var saveGame = new File ();
		saveGame.Open ("user://score.save", (File.ModeFlags) (int) File.ModeFlags.Write);

		saveGame.StoreLine (JSON.Print (currentScore));

		saveGame.Close ();

		saveGame.Open ("user://lives.save", (File.ModeFlags) (int) File.ModeFlags.Write);

		saveGame.StoreLine (JSON.Print (currentLives));

		saveGame.Close ();

		saveGame.Open ("user://health.save", (File.ModeFlags) (int) File.ModeFlags.Write);

		saveGame.StoreLine (JSON.Print (currentHealth));

		saveGame.Close ();

		saveGame.Open ("user://burst.save", (File.ModeFlags) (int) File.ModeFlags.Write);

		saveGame.StoreLine (JSON.Print (burstBulletMode));

		saveGame.Close ();
	}
}
