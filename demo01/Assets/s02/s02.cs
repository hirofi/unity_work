using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
	private int ball_count;

	private int move_count = 0;
	private int move_axis = 1;

	private float m_score = 0;

	// Use this for initialization
	void Start () {
		gm_target = new GameObject[TARGET_MAX];
		gm_target[0] = createBase(-3, 0, 1 , "target01");
		gm_target[1] = createBase(-1, 0, 1 , "target02" );
		gm_target[2] = createBase(1, 0, 1 , "target03" );
		gm_target[3] = createBase(-2, 0, 5 , "target04" );
		gm_target[4] = createBase(0, 0, 5 , "target05" );
		gm_target[5] = createBase(2, 0, 5 , "target06" );

		gm_ball = new GameObject[BALL_MAX];

	}
	
	// Update is called once per frame
	void Update () {
		frame_count = (frame_count >= 65535) ? 0 : frame_count+1;

		if (!is_already_start) {
			if (frame_count == 5) 
			{
				gm_target [0].GetComponent<target> ().anim_start ();
				gm_target [3].GetComponent<target> ().anim_start ();
			}
			if (frame_count == 10) 
			{
				gm_target [1].GetComponent<target> ().anim_start ();
				gm_target [4].GetComponent<target> ().anim_start ();
			}
			if (frame_count == 15) {
				gm_target [2].GetComponent<target> ().anim_start ();
				gm_target [5].GetComponent<target> ().anim_start ();
				is_already_start = true;
			}
		}

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

			createBall( input_mouse_pos , "ball_"+ball_count, 100.0f, 10.0f );
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

	GameObject createBase( float aX, float aY, float aZ ,string aObjName )
	{
		var mypos = transform.position;
		var addpos = new Vector3( aX, aY, aZ);
		mypos += addpos;
		
		var tg = Instantiate( Resources.Load("pf_target"), mypos, transform.rotation) as GameObject;
		tg.name = aObjName;

		target tg_cs = tg.GetComponent<target>();
		tg_cs.anim_ready();

		GameObject tgc = tg_cs.getTargetCylinder();
		target_cylinder tgc_cs = tgc.GetComponent<target_cylinder>();
		tgc_cs.onHit += hitTarget;

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

	void hitTarget( GameObject aTarget, float aScore )
	{
		m_score += aScore;

		GameObject txt_score_obj = GameObject.Find( "txtScore" );
		Text txt_score = txt_score_obj.GetComponent<Text> ();
		txt_score.text = "SCORE:"+m_score;

	}


}
