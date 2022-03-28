using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// mostly from Archanor's scifi asset pack with some modifications by me
public class SciFiBeamScript : MonoBehaviour {

    [Header("Prefabs")]
    public GameObject[] beamLineRendererPrefab;
    public GameObject[] beamStartPrefab;
    public GameObject[] beamEndPrefab;

    private int currentBeam = 0;

    private GameObject beamStart;
    private GameObject beamEnd;
    private GameObject beam;
    private LineRenderer line;

    [Header("Adjustable Variables")]
    public float beamEndOffset = 1f; //How far from the raycast hit point the end effect is positioned
    public float textureScrollSpeed = 8f; //How fast the texture scrolls along the beam
	public float textureLengthScale = 3; //Length of the beam texture

    [Header("Put Sliders here (Optional)")]
    public Slider endOffSetSlider; //Use UpdateEndOffset function on slider
    public Slider scrollSpeedSlider; //Use UpdateScrollSpeed function on slider

    [Header("Put UI Text object here to show beam name")]
    public Text textBeamName;

    public bool shooting = false;
    public Vector3 startLoc, endLoc;

    // layermask to prevent beam from colliding with IgnoreRaycast layer
    private int layermask = ~(1 << 2);

    void Start()
    {
        if (textBeamName)
            textBeamName.text = beamLineRendererPrefab[currentBeam].name;
        if (endOffSetSlider)
            endOffSetSlider.value = beamEndOffset;
        if (scrollSpeedSlider)
            scrollSpeedSlider.value = textureScrollSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (shooting){
            if (beam == null){
                //beamStart = Instantiate(beamStartPrefab[currentBeam], new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                beamEnd = Instantiate(beamEndPrefab[currentBeam], new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                beam = Instantiate(beamLineRendererPrefab[currentBeam], new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            }
            line = beam.GetComponent<LineRenderer>();

            ShootBeamInDir(startLoc, endLoc - startLoc);
        }
        else{
            //Destroy(beamStart);
            Destroy(beamEnd);
            Destroy(beam);
            //beamStart = null;
            beamEnd = null;
            beam = null;
        }
    }

    public void UpdateEndOffset()
    {
        beamEndOffset = endOffSetSlider.value;
    }

    public void UpdateScrollSpeed()
    {
        textureScrollSpeed = scrollSpeedSlider.value;
    }

    void ShootBeamInDir(Vector3 start, Vector3 dir)
    {
        #if UNITY_5_5_OR_NEWER
		line.positionCount = 2;
		#else
		line.SetVertexCount(2); 
		#endif
        line.SetPosition(0, start);
        //beamStart.transform.position = start;

        Vector3 end = Vector3.zero;
        RaycastHit hit;
        if (Physics.Raycast(start, dir, out hit, Mathf.Infinity, layermask))
            end = hit.point - (dir.normalized * beamEndOffset);
        else
            end = transform.position + (dir * 100);

        beamEnd.transform.position = end;
        line.SetPosition(1, end);

        //beamStart.transform.LookAt(beamEnd.transform.position);
        beamEnd.transform.LookAt(start);

        float distance = Vector3.Distance(start, end);
        line.sharedMaterial.mainTextureScale = new Vector2(distance / textureLengthScale, 1);
        line.sharedMaterial.mainTextureOffset -= new Vector2(Time.deltaTime * textureScrollSpeed, 0);
    }
}
//}