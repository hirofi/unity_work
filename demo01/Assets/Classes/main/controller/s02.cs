using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
//using UnityEditor;


// 他のカメラ
//position  6.16, 0.67, 0
//rotation 345, 275, 0

public class s02 : MonoBehaviour {

	// public
	public const int BALL_MAX = 100;
	public const int TARGET_MAX = 100;

	public const int MOVE_COUNT = 50;
	public const float MOVE_STEP = 0.05f;

	// private
	private GameObject[] gm_ball;
	private GameObject[] gm_target;
	private int frame_count;
	private bool is_already_start = false;
	private bool is_load_compleate = false;
	private bool is_target_create_compleate = false;

	private int ball_count;

	private int move_count = 0;
	private int move_axis = 1;

	private float m_score = 0;

	public EventManagerDynamic m_download_event_manager = null;
	public EventManagerDynamic m_target_event_manager = null;

	/// --------------
	/// マト関連
	/// --------------

	// 1).
	// マトイベントマネージャを生成し、ヒット時のターゲットを登録する
	void createTargetEventManager()
	{
		m_target_event_manager = new EventManagerDynamic ();
		m_target_event_manager.AddListener<TargetEventController> (OnHitTarget);
		m_target_event_manager.AddListener<TargetKasaEventController> (OnHitKasaTarget);
	}

	// 2).
	// マト１ヒットイベント
	public void OnHitTarget( TargetEventController aEvent )
	{
		
		m_score += aEvent.Score;
		
		GameObject txt_score_obj = GameObject.Find( "txtScore" );
		Text txt_score = txt_score_obj.GetComponent<Text> ();
		txt_score.text = "SCORE:"+m_score;
		
		Debug.Log("■OnHitTarget = " + aEvent.getHash().ToString("x4"));
	}

	// マト２ヒットイベント
	public void OnHitKasaTarget( TargetKasaEventController aEvent )
	{
		
		m_score += aEvent.Score;
		m_score += aEvent.Bonus;
		
		GameObject txt_score_obj = GameObject.Find( "txtScore" );
		Text txt_score = txt_score_obj.GetComponent<Text> ();
		txt_score.text = "SCORE:"+m_score;
		
		int k = aEvent.getHash ();
		Debug.Log("★OnHitTarget = " + aEvent.getHash().ToString("x4"));
	}



	void Awake()
	{
	}
	
	// Use this for initialization
	void Start () {

		createDownloadEventManager ();
		createTargetEventManager ();

		gm_target = new GameObject[TARGET_MAX];
		createTargetAssetBase ();

		gm_ball = new GameObject[BALL_MAX];

	}
	
	void createTarget()
	{

		// マトオブジェクトを取り出す
		target tg_cs = gm_target[0].GetComponent<target> ();
		tg_cs.anim_ready ();

		// テクスチャを張り替える
		GameObject tgc = tg_cs.getTargetCylinder ();
		
		if (tgc) {
			// マトオブジェクトからイベントを発行する為にイベントマネージャを登録
			target_cylinder tgc_cs = tgc.GetComponent<target_cylinder> ();
			tgc_cs.SetEventManager (m_target_event_manager);
		}

		if(!gm_target[1])
			gm_target[1] = createBase(-1, 0, 1 , "target02" , 1);
		if(!gm_target[2])
			gm_target[2] = createBase( 1, 0, 1 , "target03" , 1 );
		if(!gm_target[3])
			gm_target[3] = createBase(-2,1,4, "target04" , 1);
		if(!gm_target[4])
			gm_target[4] = createBase(0, 1,4, "target05" , 1);

		if(!gm_target[5])
			gm_target[5] = createBase(2, 1, 4 , "target06" , 1);

		if(!gm_target[6])
			gm_target[6] = createBase(-3, 2, 7 , "target07" , 2);

		if(!gm_target[7])
			gm_target[7] = createBase(-1, 2, 7 , "target08" , 2);

		if(!gm_target[8])
			gm_target[8] = createBase(1, 2, 7 , "target09" , 2);

	}

	void animTarget(int aFrameCount )
	{
		if (is_already_start)
			return;

		if ( aFrameCount == 5 )
		{
			gm_target [0].GetComponent<target> ().anim_start ();
			gm_target [3].GetComponent<target> ().anim_start ();
			gm_target [6].GetComponent<TargetKasaPrefab> ().anim_start ();
		}
		if ( aFrameCount == 10 )
		{
			gm_target [1].GetComponent<target> ().anim_start ();
			gm_target [4].GetComponent<target> ().anim_start ();
			gm_target [7].GetComponent<TargetKasaPrefab> ().anim_start ();
		}
		if ( aFrameCount == 15 )
		{
			gm_target [2].GetComponent<target> ().anim_start ();
			gm_target [5].GetComponent<target> ().anim_start ();
			gm_target [8].GetComponent<TargetKasaPrefab> ().anim_start ();
			is_already_start = true;
		}
	}
	
	
	// Update is called once per frame
	void Update () {

		if (is_load_compleate == false)
			return;

		// 基本バンドルターゲットを生成
		if (is_target_create_compleate == false)
		{
			createTarget ();
			is_target_create_compleate = true;
		}

		frame_count = (frame_count >= 65535) ? 0 : frame_count+1;
		animTarget ( frame_count );

		this.checkMoouseButton();
		this.checkTach();

		this.moveTarget(1);

	}

	void checkMoouseButton()
	{
		if (Input.GetMouseButtonUp(0)) {
			
			var x = Input.mousePosition.x;
			var y = Input.mousePosition.y;

			Vector2 input_mouse_pos = Input.mousePosition;

			createBall( input_mouse_pos , "ball_"+ball_count, 50.0f, 100.0f );
			ball_count++;

		}
	}

	// 
	void checkTach()
	{
		if(Input.touchCount > 0){
			if(Input.GetTouch(0).phase == TouchPhase.Began){

				var x = Input.GetTouch(0).position.x;
				var y = Input.GetTouch(0).position.y;
				ball_count++;
			}
			Camera.main.backgroundColor = Color.red;
		}
		else
		{
			Camera.main.backgroundColor = Color.blue;
		}
	}


	void moveTarget( int aType )
	{
		if ( move_count > MOVE_COUNT ) {
			move_count = 0;
			move_axis *= -1;
		}

		move_count++;

		float mv = MOVE_STEP * move_axis;
		for (int i=0; i<3; i++) {
			this.gm_target[i].transform.Translate( mv, 0, 0 );
		}

		for (int i=3; i<6; i++) {
			this.gm_target[i].transform.Translate( mv*-1, 0, 0 );
		}

	}

	private ContentsDownloadApiModel m_downloadmodel;
	GameObject createBase( float aX, float aY, float aZ ,string aObjName ,int aTargetType )
	{
		var mypos = transform.position;
		var addpos = new Vector3( aX, aY, aZ);
		mypos += addpos;

		// インスタンス生成
		BinaryAccess bin = new BinaryAccess ();
		string path = bin.getAssetPath ();

		Texture tx = null;

/*
		if (path != "") {
			string tx_path = path + "/png/kasa.png";
			tx = bin.ReadTexture (tx_path, 10, 10);
		}
*/
		GameObject tg = null;
		if (aTargetType == 1) {
			tg = Instantiate (Resources.Load ("pf_target"), mypos, transform.rotation) as GameObject;

			// マトオブジェクトを取り出す
			target tg_cs = tg.GetComponent<target> ();
			tg_cs.anim_ready ();

			// テクスチャを張り替える
			GameObject tgc = tg_cs.getTargetCylinder ();

			// マトオブジェクトからイベントを発行する為にイベントマネージャを登録
			target_cylinder tgc_cs = tgc.GetComponent<target_cylinder> ();
			tgc_cs.SetEventManager( m_target_event_manager );

			
			if (tx)
				tgc.GetComponent<Renderer> ().material.mainTexture = tx;

			tg.name = aObjName;
		} else if (aTargetType == 2) {
			tg = Instantiate (Resources.Load ("target_kasa_prefab"), mypos, transform.rotation) as GameObject;

			// マトオブジェクトからイベントを発行する為にイベントマネージャを登録
			TargetKasaPrefab tg_cs = tg.GetComponent<TargetKasaPrefab> ();
			tg_cs.SetEventManager( m_target_event_manager );

			tg.transform.rotation = Quaternion.FromToRotation (transform.up, transform.up);
			tg.name = aObjName;
		}

		return tg;
	}

	/// --------------
	/// ダウンロード関連
	/// --------------

	// 1).
	// ダウンロード用のイベントマネージャを生成し、完了時関数を登録する。
	void createDownloadEventManager()
	{
		m_download_event_manager = new EventManagerDynamic ();
		m_download_event_manager.AddListener<DownloadEventController> (OnCompleateDownload);
	}

	// 2).
	// ダウンロード完了時に呼び出されるメソッド
	public void OnCompleateDownload( DownloadEventController aEvent )
	{
		if (aEvent.Content.Download_Status == ContentsDownloadApiModel.enmDownloadStatus.ERROR_EXIT) {
			Debug.Log("エラー : ");
			return;
		}
		gm_target[ aEvent.Token.Index ] = Instantiate( aEvent.Content.GameObjectData );
		gm_target[aEvent.Token.Index].name = aEvent.Token.Name;
		gm_target[aEvent.Token.Index].transform.position = aEvent.Token.Postion;
	
		// マトオブジェクトを取り出す
		target tg_cs = gm_target[ aEvent.Token.Index ].GetComponent<target> ();
		tg_cs.anim_ready ();

		// テクスチャを張り替えてみる
		GameObject tgc = tg_cs.getTargetCylinder ();
		if(tgc)
		{
			// マトオブジェクトからイベントを発行する為にイベントマネージャを登録
			target_cylinder tgc_cs = tgc.GetComponent<target_cylinder> ();
			tgc_cs.SetEventManager( m_target_event_manager );

			if ( aEvent.Token.Textuer )
				tgc.GetComponent<Renderer> ().material.mainTexture = aEvent.Token.Textuer;
		}


		is_load_compleate = true;
		Debug.Log ("downloaded. path=" + aEvent.getLocalFilePath());
	}

	// 3).
	// ダウンロード後のターゲット作成
	void createTargetAssetBase()
	{
		createAssetBandle(-3, 0, 1 , "target01" , 3 , 0 );
	}

	// アセットバンドルをダウンロードする
	GameObject createAssetBandle( float aX, float aY, float aZ ,string aObjName ,int aTargetType, int aGmTargetNum )
	{
		var mypos = transform.position;
		var addpos = new Vector3( aX, aY, aZ);
		mypos += addpos;
		
		// インスタンス生成
		BinaryAccess bin = new BinaryAccess ();
		string path = bin.getAssetPath ();

		GameObject tg = null;

		// トークンを生成（ダウンロード後、イベント通知に付帯する情報）
		DownloadToken token = new DownloadToken ();
		token.Postion = mypos;
		token.Index = aGmTargetNum;
		token.Name = aObjName;
		if (path != "") {
			string tx_path = path + "/png/kasa.png";
			token.Textuer = bin.ReadTexture (tx_path, 10, 10);
		}

		// ダウンロード
		DownloadController download = new DownloadController ();
		download.SetEventManager (m_download_event_manager);
		download.FileName = "pf_target";
		download.DomainName = "pshpz01.isl.gacha.fujitv.co.jp/unity/";
		download.Token = token;
		download.StartDownload();

		return tg;
	}




	// Touch point 
	//  (0,1)   (1,1)
	//     +---+
	//     |   |
	//     +---+
	// (0,0)   (1,0)

	public Camera m_setCamera = null;
	private const float TOUCH_PANEL_CONVERT_SCALE = 0.5f;
	GameObject createBall( Vector2 aInputPosition, string aObjName, float aShotForce, float aMoveSpeed )
	{

		if (m_setCamera == null)
			m_setCamera = Camera.main;

		// 玉のスタート位置を補正(カメラ位置から補正する）
		Vector3 start_pos = m_setCamera.transform.position;
		start_pos.y = 1.0f;
		start_pos.z = -1.0f;

		// パネルタッチ位置を縮尺して奥の方にターゲットを移動する
		Vector2 scaled_touch = touchScaleConvert (aInputPosition , TOUCH_PANEL_CONVERT_SCALE );
		Vector3 scaled_panel = new Vector3( scaled_touch.x, scaled_touch.y, 2.0f);

		// ターゲット位置をワールド座標に変換
		Vector3 target_point = m_setCamera.ScreenToWorldPoint( scaled_panel );

		// 玉のスタート位置がらターゲットの位置も補正する
		target_point.y -= (start_pos.y * TOUCH_PANEL_CONVERT_SCALE );
		target_point.z = 1f;

		Quaternion rot = m_setCamera.transform.rotation;

		var _ball = Instantiate( Resources.Load("pf_ball"), start_pos, rot ) as GameObject;
		_ball.name = aObjName;
		
		ball ball_cs = _ball.GetComponent<ball>();
		ball_cs.init( target_point, aShotForce, aMoveSpeed );
		
		return _ball;
	}

	Vector2 touchScaleConvert( Vector2 aPosition, float aScale )
	{
		if (aScale > 1)
			aScale = 1;

		Vector2 scr_position = aPosition * aScale;

		Vector2 scr_screen;
		scr_screen.x = Screen.width * aScale;
		scr_screen.y = Screen.height * aScale;

		Vector2 scr_center_offset;
		scr_center_offset.x = (Screen.width - scr_screen.x) / 2;
		scr_center_offset.y = (Screen.height - scr_screen.y) / 2;

		scr_position += scr_center_offset;

		return scr_position;
	}




}

