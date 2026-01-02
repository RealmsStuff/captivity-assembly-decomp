using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;
using Mono.Data.Sqlite;
using UnityEngine;

public static class ManagerDB
{
	public delegate void DelInteraction(int i_idInteraction, NPC i_npc);

	public delegate void DelStartGame();

	private static SqliteConnection m_con = new SqliteConnection();

	private static string m_pathToSaveFile;

	private static string m_pathToSaveFileUri;

	public static event DelInteraction OnInteraction;

	public static event DelStartGame OnStartGame;

	public static void Initialize()
	{
		m_pathToSaveFile = System.IO.Path.Combine(Application.persistentDataPath, "xGameDB.db");
		m_pathToSaveFileUri = "URI=file:" + m_pathToSaveFile;
		CheckForSaveFile();
	}

	private static void OpenCon()
	{
		m_con = new SqliteConnection(m_pathToSaveFileUri);
		m_con.Open();
	}

	private static void CloseCon()
	{
		m_con.Close();
	}

	private static void CheckForSaveFile()
	{
		if (!File.Exists(m_pathToSaveFile))
		{
			CreateSaveFile();
		}
	}

	public static int ExecuteNonQuery(string i_query)
	{
		if (m_con.State != ConnectionState.Open)
		{
			OpenCon();
		}
		SqliteCommand sqliteCommand = m_con.CreateCommand();
		sqliteCommand.CommandText = i_query;
		return ExecuteNonQuery(sqliteCommand);
	}

	private static int ExecuteNonQuery(SqliteCommand i_cmd)
	{
		return i_cmd.ExecuteNonQuery();
	}

	private static async Task<int> ExecuteNonQueryAsync(string i_query)
	{
		if (m_con.State != ConnectionState.Open)
		{
			OpenCon();
		}
		SqliteCommand sqliteCommand = m_con.CreateCommand();
		sqliteCommand.CommandText = i_query;
		return await ExecuteNonQueryAsync(sqliteCommand);
	}

	private static Task<int> ExecuteNonQueryAsync(SqliteCommand i_cmd)
	{
		return Task.Run(() => i_cmd.ExecuteNonQueryAsync());
	}

	private static async Task<DbDataReader> ExecuteReaderAsync(string i_query)
	{
		if (m_con.State != ConnectionState.Open)
		{
			OpenCon();
		}
		SqliteCommand sqliteCommand = m_con.CreateCommand();
		sqliteCommand.CommandText = i_query;
		return await ExecuteReaderAsync(sqliteCommand);
	}

	private static Task<DbDataReader> ExecuteReaderAsync(SqliteCommand i_cmd)
	{
		return Task.Run(() => i_cmd.ExecuteReaderAsync());
	}

	public static DbDataReader ExecuteReader(string i_query)
	{
		if (m_con.State != ConnectionState.Open)
		{
			OpenCon();
		}
		SqliteCommand sqliteCommand = m_con.CreateCommand();
		sqliteCommand.CommandText = i_query;
		return ExecuteReader(sqliteCommand);
	}

	private static DbDataReader ExecuteReader(SqliteCommand i_cmd)
	{
		return i_cmd.ExecuteReader();
	}

	public static object GetExecuteReader(string i_query, int i_ordinal)
	{
		DbDataReader dbDataReader = ExecuteReader(i_query);
		object result = null;
		while (dbDataReader.Read())
		{
			result = dbDataReader.GetValue(i_ordinal);
		}
		return result;
	}

	public static object GetExecuteReader(string i_query, string i_column)
	{
		DbDataReader dbDataReader = ExecuteReader(i_query);
		object result = null;
		while (dbDataReader.Read())
		{
			result = dbDataReader[i_column];
		}
		return result;
	}

	public static async void Rape(NPC i_npc)
	{
		await ExecuteReaderAsync("Update tbl_relationship SET timesRaped = timesRaped + '1' WHERE idNpc = " + i_npc.GetId());
		if (await IsPlayerVirgin())
		{
			Deflower(i_npc);
		}
	}

	private static async Task<bool> IsPlayerVirgin()
	{
		DbDataReader dbDataReader = await ExecuteReaderAsync("SELECT idNpcDeflower FROM tbl_player WHERE idPlayer = 1");
		while (dbDataReader.Read())
		{
			if (dbDataReader["idNpcDeflower"] == DBNull.Value)
			{
				return true;
			}
		}
		return false;
	}

	public static int GetTimesRaped()
	{
		DbDataReader dbDataReader = ExecuteReader("SELECT SUM(timesRaped) FROM tbl_relationship WHERE idPlayer = 1");
		int result = -1;
		while (dbDataReader.Read())
		{
			result = Convert.ToInt32(dbDataReader[0]);
		}
		return result;
	}

	private static async void Deflower(NPC i_npc)
	{
		await ExecuteNonQueryAsync("UPDATE tbl_player SET idNpcDeflower = " + i_npc.GetId() + " WHERE idPlayer = 1");
	}

	public static async void MindBreak(NPC i_npc)
	{
		await ExecuteNonQueryAsync("UPDATE tbl_relationship SET timesMindBroken = timesMindBroken + 1 WHERE idNpc = " + i_npc.GetId());
	}

	public static int GetTimesMindBroken()
	{
		DbDataReader dbDataReader = ExecuteReader("SELECT SUM(timesMindBroken) FROM tbl_relationship WHERE idPlayer = 1");
		int result = -1;
		while (dbDataReader.Read())
		{
			result = Convert.ToInt32(dbDataReader[0]);
		}
		return result;
	}

	public static async Task AddNpcs(List<NPC> i_npcs)
	{
		foreach (NPC i_npc in i_npcs)
		{
			if (i_npc.GetId() != -1)
			{
				await AddNpcIfNotExists(i_npc);
			}
		}
	}

	private static async Task AddNpc(NPC i_npc)
	{
		await ExecuteNonQueryAsync("INSERT INTO tbl_npc VALUES (" + i_npc.GetId() + ", \"" + i_npc.GetName() + "\")");
	}

	private static async Task AddRelationship(NPC i_npc)
	{
		await ExecuteNonQueryAsync("INSERT INTO tbl_relationship (idPlayer, idNpc)VALUES (1, " + i_npc.GetId() + ")");
	}

	public static Relationship GetRelationShip(NPC i_npc)
	{
		DbDataReader dbDataReader = ExecuteReader("SELECT * FROM tbl_relationship WHERE idPlayer = 1 AND idNpc = " + i_npc.GetId());
		if (dbDataReader.Read())
		{
			return new Relationship
			{
				Id = Convert.ToInt32(dbDataReader["idRelationship"]),
				IdPlayer = Convert.ToInt32(dbDataReader["idPlayer"]),
				IdNpc = Convert.ToInt32(dbDataReader["idNpc"]),
				TimesKilled = Convert.ToInt32(dbDataReader["timesKilled"]),
				DamageTaken = Convert.ToInt32(dbDataReader["damageTaken"]),
				TimesKO = Convert.ToInt32(dbDataReader["timesKo"]),
				TimesRaped = Convert.ToInt32(dbDataReader["timesRaped"]),
				TimesOrgasmed = Convert.ToInt32(dbDataReader["timesOrgasmed"]),
				LitreCumTaken = Convert.ToDecimal(dbDataReader["litreCumTaken"]),
				NumOfFetusInserted = Convert.ToInt32(dbDataReader["numOfFetusInserted"])
			};
		}
		return null;
	}

	public static List<Relationship> GetAllRelationShips()
	{
		DbDataReader dbDataReader = ExecuteReader("SELECT * FROM tbl_relationship WHERE idPlayer = 1");
		List<Relationship> list = new List<Relationship>();
		while (dbDataReader.Read())
		{
			Relationship relationship = new Relationship();
			relationship.Id = Convert.ToInt32(dbDataReader["idRelationship"]);
			relationship.IdPlayer = Convert.ToInt32(dbDataReader["idPlayer"]);
			relationship.IdNpc = Convert.ToInt32(dbDataReader["idNpc"]);
			relationship.TimesKilled = Convert.ToInt32(dbDataReader["timesKilled"]);
			relationship.DamageTaken = Convert.ToInt32(dbDataReader["damageTaken"]);
			relationship.TimesKO = Convert.ToInt32(dbDataReader["timesKo"]);
			relationship.TimesRaped = Convert.ToInt32(dbDataReader["timesRaped"]);
			relationship.TimesOrgasmed = Convert.ToInt32(dbDataReader["timesOrgasmed"]);
			relationship.LitreCumTaken = Convert.ToDecimal(dbDataReader["litreCumTaken"]);
			relationship.NumOfFetusInserted = Convert.ToInt32(dbDataReader["numOfFetusInserted"]);
			list.Add(relationship);
		}
		return list;
	}

	public static async void Kill(NPC i_npcKilled)
	{
		await ExecuteNonQueryAsync("UPDATE tbl_relationship SET timesKilled = timesKilled + '1' WHERE idNpc = " + i_npcKilled.GetId());
	}

	public static int GetKillsTotal()
	{
		DbDataReader dbDataReader = ExecuteReader("SELECT SUM(timesKilled) FROM tbl_relationship WHERE idPlayer = 1");
		int result = -1;
		while (dbDataReader.Read())
		{
			result = Convert.ToInt32(dbDataReader[0]);
		}
		return result;
	}

	public static async void Orgasm(NPC i_npc)
	{
		await ExecuteNonQueryAsync("UPDATE tbl_relationship SET timesOrgasmed = timesOrgasmed + '1' WHERE idNpc = " + i_npc.GetId());
	}

	public static async void AddLitreCumTaken(NPC i_npc, float i_litreCum)
	{
		string text = Math.Round((decimal)i_litreCum, 3, MidpointRounding.AwayFromZero).ToString();
		text = text.Replace(',', '.');
		await ExecuteNonQueryAsync("UPDATE tbl_relationship SET litreCumTaken = litreCumTaken + " + text + " WHERE idNpc = '" + i_npc.GetId() + "'");
	}

	public static async Task<decimal> GetLitreCumTaken()
	{
		DbDataReader dbDataReader = await ExecuteReaderAsync("SELECT SUM(litreCumTaken) FROM tbl_relationship WHERE idPlayer = 1");
		decimal result = -1m;
		while (dbDataReader.Read())
		{
			result = Convert.ToDecimal(dbDataReader[0]);
		}
		return result;
	}

	public static async void KO(NPC i_npc)
	{
		await ExecuteNonQueryAsync("UPDATE tbl_relationship SET timesKO = timesKO + '1' WHERE idNpc = '" + i_npc.GetId() + "'");
	}

	public static async void Interaction(int i_idInteraction, NPC i_npc)
	{
		ManagerDB.OnInteraction?.Invoke(i_idInteraction, i_npc);
		if (!(await ExecuteReaderAsync("SELECT idInteractionRelationship FROM tbl_interactionRelationship WHERE idInteraction = " + i_idInteraction)).HasRows)
		{
			await AddInteractionRelationship(i_idInteraction, i_npc.GetId());
		}
		await ExecuteReaderAsync("UPDATE tbl_interactionRelationship SET timesInteraction = timesInteraction + '1' WHERE idNpc = '" + i_npc.GetId() + "' AND idInteraction = '" + i_idInteraction + "'");
	}

	public static async Task AddInteractions(List<Interaction> i_interactions)
	{
		foreach (Interaction i_interaction in i_interactions)
		{
			await AddInteractionIfNotExists(i_interaction);
		}
	}

	private static async Task AddInteractionIfNotExists(Interaction i_interaction)
	{
		if (!(await ExecuteReaderAsync("SELECT idInteraction FROM tbl_interaction WHERE idInteraction = " + i_interaction.GetId())).HasRows)
		{
			await ExecuteNonQueryAsync("INSERT INTO tbl_interaction (idInteraction, nameInteraction, descriptionInteraction) VALUES (\"" + i_interaction.GetId() + "\", \"" + i_interaction.GetName() + "\", \"" + i_interaction.GetDescription() + "\")");
		}
	}

	private static async Task AddInteractionRelationship(int i_idInteraction, int i_idNpc)
	{
		await ExecuteNonQueryAsync("INSERT INTO tbl_interactionRelationship (idInteraction, idPlayer, idNpc) VALUES (" + i_idInteraction + ", " + 1 + ", " + i_idNpc + ")");
	}

	public static async void AddFetusCount(NPC i_npcParent)
	{
		await ExecuteNonQueryAsync("UPDATE tbl_relationship SET numOfFetusInserted = numOfFetusInserted + '1' WHERE idNpc = " + i_npcParent.GetId());
	}

	public static async void Birth(Fetus i_fetusBirthed, NPC i_npcOffspring)
	{
		NPC l_npcParent = i_fetusBirthed.GetNpcParent();
		if (i_npcOffspring.GetId() != -1 && l_npcParent.GetId() != -1)
		{
			string l_query = ((!l_npcParent) ? ("SELECT idBirth FROM tbl_birth WHERE idNpc = NULL AND idNpcOffspring = " + i_npcOffspring.GetId()) : ("SELECT idBirth FROM tbl_birth WHERE idNpc = " + i_fetusBirthed.GetNpcParent().GetId() + " AND idNpcOffspring = " + i_npcOffspring.GetId()));
			if (!(await ExecuteReaderAsync(l_query)).HasRows)
			{
				await AddBirth(i_fetusBirthed, i_npcOffspring);
			}
			DbDataReader dbDataReader = await ExecuteReaderAsync(l_query);
			while (dbDataReader.Read())
			{
				dbDataReader.GetInt32(0);
			}
			l_query = ((!l_npcParent) ? ("UPDATE tbl_birth SET timesBirth = timesBirth + '1' WHERE idNpcOffspring = " + i_npcOffspring.GetId() + " AND idNpc = NULL") : ("UPDATE tbl_birth SET timesBirth = timesBirth + '1' WHERE idNpcOffspring = " + i_npcOffspring.GetId() + " AND idNpc = " + i_fetusBirthed.GetNpcParent().GetId()));
			await ExecuteNonQueryAsync(l_query);
		}
	}

	private static async Task AddBirth(Fetus i_fetusBirthed, NPC i_npcOffspring)
	{
		int num = 0;
		if (i_fetusBirthed.GetActorsInFetus()[0] is Egg)
		{
			num = 1;
		}
		string i_query = ((!i_fetusBirthed.GetNpcParent()) ? ("INSERT INTO tbl_birth (idNpcOffspring, idPlayer, idNpc, isEgg) VALUES (" + i_npcOffspring.GetId() + ", " + 1 + ", NULL, " + num + ")") : ("INSERT INTO tbl_birth (idNpcOffspring, idPlayer, idNpc, isEgg) VALUES (" + i_npcOffspring.GetId() + ", " + 1 + ", " + i_fetusBirthed.GetNpcParent().GetId() + ", " + num + ")"));
		await ExecuteNonQueryAsync(i_query);
	}

	public static async Task<int> GetTimesBirth()
	{
		DbDataReader dbDataReader = await ExecuteReaderAsync("SELECT SUM(timesBirth) FROM tbl_birth WHERE idPlayer = 1");
		int result = -1;
		while (dbDataReader.Read())
		{
			try
			{
				result = Convert.ToInt32(dbDataReader[0]);
			}
			catch
			{
				result = 0;
			}
		}
		return result;
	}

	private static async Task AddNpcIfNotExists(NPC i_npc)
	{
		if (!IsNpcExists(i_npc))
		{
			await AddNpc(i_npc);
		}
		if (!ExecuteReader("SELECT * FROM tbl_relationship WHERE idNpc = " + i_npc.GetId()).HasRows)
		{
			await AddRelationship(i_npc);
		}
	}

	private static bool IsNpcExists(NPC i_npc)
	{
		if (ExecuteReader("SELECT idNpc FROM tbl_npc WHERE idNpc = " + i_npc.GetId()).HasRows)
		{
			return true;
		}
		return false;
	}

	public static async void AddDamageTaken(NPC i_npc, int i_damageTakenToAdd)
	{
		await ExecuteNonQueryAsync("UPDATE tbl_relationship SET damageTaken = damageTaken + " + i_damageTakenToAdd + " WHERE idNpc = " + i_npc.GetId());
	}

	public static async Task AddClothes(List<Clothing> i_clothes)
	{
		foreach (Clothing i_clothe in i_clothes)
		{
			await AddClothingIfNotExists(i_clothe);
		}
	}

	private static async Task AddClothingIfNotExists(Clothing i_clothing)
	{
		if (!IsClothingExists(i_clothing))
		{
			await AddClothing(i_clothing);
		}
	}

	private static async Task AddClothing(Clothing i_clothing)
	{
		await ExecuteNonQueryAsync("INSERT INTO tbl_clothing VALUES (" + i_clothing.GetId() + ", 0, 0)");
	}

	private static bool IsClothingExists(Clothing i_clothing)
	{
		if (ExecuteReader("SELECT * FROM tbl_clothing WHERE idClothing = " + i_clothing.GetId()).HasRows)
		{
			return true;
		}
		return false;
	}

	public static List<int> GetIdsUnlockedClothes()
	{
		DbDataReader dbDataReader = ExecuteReader("SELECT * FROM tbl_clothing WHERE isUnlocked = 1");
		List<int> list = new List<int>();
		while (dbDataReader.Read())
		{
			list.Add(dbDataReader.GetInt32(0));
		}
		return list;
	}

	public static void EquipClothes(List<Clothing> i_clothes)
	{
		ExecuteNonQuery("UPDATE tbl_clothing SET isEquipped = 0");
		foreach (Clothing i_clothe in i_clothes)
		{
			ExecuteNonQuery("UPDATE tbl_clothing SET isEquipped = 1 WHERE idClothing = " + i_clothe.GetId());
		}
	}

	public static List<int> GetIdsEquippedClothes()
	{
		DbDataReader dbDataReader = ExecuteReader("SELECT idClothing FROM tbl_clothing WHERE isUnlocked = 1 AND isEquipped = 1");
		List<int> list = new List<int>();
		while (dbDataReader.Read())
		{
			list.Add(dbDataReader.GetInt32(0));
		}
		return list;
	}

	public static async void UnlockClothing(Clothing i_clothing)
	{
		await ExecuteNonQueryAsync("UPDATE tbl_clothing SET isUnlocked = 1 WHERE idClothing = " + i_clothing.GetId());
	}

	public static async void UnlockAllClothes()
	{
		await ExecuteNonQueryAsync("UPDATE tbl_clothing SET isUnlocked = 1");
	}

	public static async void SetSkinColor(SkinColor i_skinColor)
	{
		int num = 1;
		switch (i_skinColor)
		{
		case SkinColor.Pale:
			num = 0;
			break;
		case SkinColor.White:
			num = 1;
			break;
		case SkinColor.Tan:
			num = 2;
			break;
		case SkinColor.Black:
			num = 3;
			break;
		}
		await ExecuteNonQueryAsync("UPDATE tbl_player SET skinColor = " + num);
	}

	public static SkinColor GetSkinColorPlayer()
	{
		DbDataReader dbDataReader = ExecuteReader("SELECT skinColor FROM tbl_player WHERE idPlayer = 1");
		int num = 1;
		while (dbDataReader.Read())
		{
			num = dbDataReader.GetInt32(0);
		}
		SkinColor result = SkinColor.White;
		switch (num)
		{
		case 0:
			result = SkinColor.Pale;
			break;
		case 1:
			result = SkinColor.White;
			break;
		case 2:
			result = SkinColor.Tan;
			break;
		case 3:
			result = SkinColor.Black;
			break;
		}
		return result;
	}

	public static async void SetEyeColor(EyeColor i_eyeColor)
	{
		int num = 0;
		switch (i_eyeColor)
		{
		case EyeColor.Blue:
			num = 0;
			break;
		case EyeColor.Brown:
			num = 1;
			break;
		case EyeColor.Green:
			num = 2;
			break;
		case EyeColor.Yellow:
			num = 3;
			break;
		}
		await ExecuteNonQueryAsync("UPDATE tbl_player SET eyeColor = " + num);
	}

	public static EyeColor GetEyeColorPlayer()
	{
		DbDataReader dbDataReader = ExecuteReader("SELECT eyeColor FROM tbl_player WHERE idPlayer = 1");
		int num = 0;
		while (dbDataReader.Read())
		{
			num = dbDataReader.GetInt32(0);
		}
		EyeColor result = EyeColor.Blue;
		switch (num)
		{
		case 0:
			result = EyeColor.Blue;
			break;
		case 1:
			result = EyeColor.Brown;
			break;
		case 2:
			result = EyeColor.Green;
			break;
		case 3:
			result = EyeColor.Yellow;
			break;
		}
		return result;
	}

	public static async Task AddChallenges(List<Challenge> i_challenges)
	{
		foreach (Challenge i_challenge in i_challenges)
		{
			await AddChallengeIfNotExists(i_challenge);
		}
	}

	private static async Task AddChallengeIfNotExists(Challenge i_challenge)
	{
		if (!(await IsChallengeExists(i_challenge)))
		{
			await AddChallenge(i_challenge);
		}
	}

	private static async Task AddChallenge(Challenge i_challenge)
	{
		await ExecuteNonQueryAsync("INSERT INTO tbl_challenge VALUES (" + i_challenge.GetId() + ", 0)");
	}

	private static async Task<bool> IsChallengeExists(Challenge i_challenge)
	{
		if ((await ExecuteReaderAsync("SELECT * FROM tbl_challenge WHERE idChallenge = " + i_challenge.GetId())).HasRows)
		{
			return true;
		}
		return false;
	}

	public static List<int> GetIdsOpenChallenges()
	{
		DbDataReader dbDataReader = ExecuteReader("SELECT idChallenge FROM tbl_challenge WHERE stateChallenge = 0");
		List<int> list = new List<int>();
		while (dbDataReader.Read())
		{
			list.Add(dbDataReader.GetInt32(0));
		}
		return list;
	}

	public static List<int> GetIdsCompletedPendingChallenges()
	{
		DbDataReader dbDataReader = ExecuteReader("SELECT idChallenge FROM tbl_challenge WHERE stateChallenge = 1");
		List<int> list = new List<int>();
		while (dbDataReader.Read())
		{
			list.Add(dbDataReader.GetInt32(0));
		}
		return list;
	}

	public static List<int> GetIdsCompletedSeenChallenges()
	{
		DbDataReader dbDataReader = ExecuteReader("SELECT idChallenge FROM tbl_challenge WHERE stateChallenge = 2");
		List<int> list = new List<int>();
		while (dbDataReader.Read())
		{
			list.Add(dbDataReader.GetInt32(0));
		}
		return list;
	}

	public static async void CompleteChallenge(Challenge i_challenge)
	{
		await ExecuteNonQueryAsync("UPDATE tbl_challenge SET stateChallenge = 1 WHERE idChallenge = " + i_challenge.GetId());
	}

	public static void AddStages(List<Stage> i_stages)
	{
		foreach (Stage i_stage in i_stages)
		{
			AddStageIfNotExists(i_stage);
		}
	}

	private static void AddStageIfNotExists(Stage i_stage)
	{
		if (!IsStageExists(i_stage))
		{
			AddStage(i_stage);
		}
	}

	private static void AddStage(Stage i_stage)
	{
		ExecuteNonQuery("INSERT INTO tbl_stage VALUES (" + i_stage.GetId() + ", 0)");
	}

	private static bool IsStageExists(Stage i_stage)
	{
		if (ExecuteReader("SELECT * FROM tbl_stage WHERE idStage = " + i_stage.GetId()).HasRows)
		{
			return true;
		}
		return false;
	}

	public static async void SetChallengeToSeen(Challenge i_challenge)
	{
		await ExecuteNonQueryAsync("UPDATE tbl_challenge SET stateChallenge = 2 WHERE idChallenge = " + i_challenge.GetId());
	}

	public static int GetHighscore(Stage i_stage)
	{
		DbDataReader dbDataReader = ExecuteReader("SELECT highscore FROM tbl_stage WHERE idStage = " + i_stage.GetId());
		int result = 0;
		while (dbDataReader.Read())
		{
			result = dbDataReader.GetInt32(0);
		}
		return result;
	}

	public static async void SetHighscore(Stage i_stage, int i_highScoreNew)
	{
		await ExecuteNonQueryAsync("UPDATE tbl_stage SET highscore = " + i_highScoreNew + " WHERE idStage = " + i_stage.GetId());
	}

	public static bool IsFirstTimeStart()
	{
		DbDataReader dbDataReader = ExecuteReader("SELECT isFirstTimeStart FROM tbl_player WHERE idPlayer = 1");
		int num = -1;
		while (dbDataReader.Read())
		{
			num = dbDataReader.GetInt32(0);
		}
		if (num == 1)
		{
			return true;
		}
		return false;
	}

	public static async void FirstTimeStart()
	{
		await ExecuteNonQueryAsync("UPDATE tbl_player SET isFirstTimeStart = 0 WHERE idPlayer = 1");
		if (!IsInputFilled())
		{
			ResetInput();
			CommonReferences.Instance.GetManagerInput().SetButtonsToDefault();
		}
	}

	public static string GetCodeKeypad()
	{
		DbDataReader dbDataReader = ExecuteReader("SELECT codeKeypad FROM tbl_player WHERE idPlayer = 1");
		string result = "";
		while (dbDataReader.Read())
		{
			result = dbDataReader[0].ToString();
		}
		return result;
	}

	public static void ResetSave()
	{
		int[] array = new int[4];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = UnityEngine.Random.Range(0, 9);
		}
		string text = array[0].ToString() + array[1] + array[2] + array[3];
		ExecuteNonQuery("DELETE FROM tbl_interaction;\r\n        DELETE FROM tbl_interactionRelationship;\r\n        DELETE FROM tbl_npc;\r\n        DELETE FROM tbl_relationship;\r\n        DELETE FROM tbl_birth;\r\n        DELETE FROM tbl_clothing;\r\n        DELETE FROM tbl_challenge;\r\n        DELETE FROM tbl_stage;\r\n        DELETE FROM tbl_player;\r\n        INSERT INTO tbl_player (idPlayer, isFirstTimeStart, codeKeypad) VALUES (1, 1, " + text + ");");
		ResetVolumes();
		SetDifficulty(Difficulty.Normal);
		SetIsReduceGunFlash(i_isReduce: false);
	}

	public static void ResetVolumes()
	{
		PlayerPrefs.SetInt("VolumeMaster", 10);
		PlayerPrefs.SetInt("VolumeMusic", 8);
		PlayerPrefs.SetInt("VolumeAmbience", 5);
		PlayerPrefs.SetInt("VolumeVoice", 7);
		PlayerPrefs.SetInt("VolumeSFX", 6);
		PlayerPrefs.SetInt("VolumeHitsound", 5);
	}

	public static void SetDifficulty(Difficulty i_difficulty)
	{
		PlayerPrefs.SetString("Difficulty", i_difficulty.ToString());
	}

	public static void SetIsReduceGunFlash(bool i_isReduce)
	{
		if (i_isReduce)
		{
			PlayerPrefs.SetInt("IsReduceGunFlash", 1);
		}
		else
		{
			PlayerPrefs.SetInt("IsReduceGunFlash", 0);
		}
	}

	public static string GetDifficulty()
	{
		return PlayerPrefs.GetString("Difficulty");
	}

	public static bool IsReduceGunFlash()
	{
		if (PlayerPrefs.GetInt("IsReduceGunFlash") == 1)
		{
			return true;
		}
		return false;
	}

	public static async void StartGame()
	{
		await AddChallenges(CommonReferences.Instance.GetManagerChallenge().GetAllChallenges());
		CommonReferences.Instance.GetManagerChallenge().UpdateChallenges();
		await AddClothes(Library.Instance.Clothes.GetAllClothes());
		Library.Instance.Clothes.FirstTimeStart();
		await AddNpcs(Library.Instance.Actors.GetAllNpcs());
		await AddInteractions(Library.Instance.Actors.GetAllInteractions());
		ManagerDB.OnStartGame?.Invoke();
	}

	public static bool IsInputFilled()
	{
		return ExecuteReader("SELECT * FROM tbl_input").HasRows;
	}

	public static void ResetInput()
	{
		string i_query = "DELETE FROM tbl_input";
		ExecuteNonQuery(i_query);
		ManagerInput managerInput = CommonReferences.Instance.GetManagerInput();
		List<string> list = new List<string>();
		for (int i = 0; i < managerInput.GetButtonsDefault().Count; i++)
		{
			i_query = "INSERT INTO tbl_input VALUES ";
			i_query += "(\"";
			i_query = i_query + managerInput.GetButtonsDefault()[i].GetName() + "\", \"" + managerInput.GetButtonsDefault()[i].GetKeyCode();
			i_query += "\")";
			i_query += ";";
			list.Add(i_query);
		}
		for (int j = 0; j < list.Count; j++)
		{
			ExecuteNonQuery(list[j]);
		}
	}

	public static List<InputButtonXGame> GetInputButtons()
	{
		DbDataReader dbDataReader = ExecuteReader("SELECT * FROM tbl_input");
		List<InputButtonXGame> list = new List<InputButtonXGame>();
		while (dbDataReader.Read())
		{
			list.Add(new InputButtonXGame((string)dbDataReader["nameKey"], (string)dbDataReader["keyCode"]));
		}
		return list;
	}

	public static void SetInputButton(string i_nameButton, KeyCode i_keyCodeToSet)
	{
		ExecuteNonQuery("UPDATE tbl_input SET keyCode = '" + i_keyCodeToSet.ToString() + "' WHERE nameKey = '" + i_nameButton + "'");
	}

	private static void CreateSaveFile()
	{
		string path = System.IO.Path.Combine(Application.streamingAssetsPath, "dbScript.sql");
		OpenCon();
		ExecuteNonQuery(File.ReadAllText(path));
		ResetSave();
		CloseCon();
	}
}
