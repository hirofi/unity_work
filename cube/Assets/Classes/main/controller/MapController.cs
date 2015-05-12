using UnityEngine;
using System.Collections;

public class MapController /*: MonoBehaviour*/ {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	private MapModel m_map_model = null;
	public MapController()
	{
		m_map_model = new MapModel ();
	}


	public void f_CreateMap( int aStageNum )
	{
		Vector3 map_range_from = new Vector3(0,0,0);
		Vector3 map_range_to = new Vector3(0,0,0);

		m_map_model.f_LoadMapData( map_range_from, map_range_to );
	}

	public void f_DrawMap()
	{
		int a;
		a = 0;
	}
}
