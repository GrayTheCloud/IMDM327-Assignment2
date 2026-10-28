// IMDM327 Material
// Use CSV or JSON to load data into the simulation. Both formats are supported, but they use different data types. 
// The CSV format uses a struct, while the JSON format uses a class. This script demonstrates how to load both formats and access their data.
using Mediapipe.Unity;
using UnityEditor.Profiling;
using UnityEngine;
using UnityEngine.UI;
public class SolarSystemStarter : MonoBehaviour
{
    public Button playbutton;
    // These components can be attached independently.
    DataCSV solarCSV;
    DataJSON solarJSON;
    const float G = 6.674e-11f; // Gravitational constant
    public float TimeController = 10000000f;

    public float lightintensity = 10f;
    PlanetProperty[] planetProperties;
    private int numberOfSphere = 10;
    
    private bool playing = false;

    public float playrange = .25f;

    public Note[] key =
    {
        new Note ("A", 3),
        new Note("B", 3),
        new Note("C#", 4),
        new Note("D#", 4),
        new Note("E", 4),
        new Note("F#", 4),
        new Note("G#", 4),
        new Note("A",4),
        new Note("B", 4),
        new Note("C#", 5) 
    };

    public Slider[] anglesliders;


    public PlanetSynth synth;
    class PlanetProperty // why struct?
    {                   // https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/choosing-between-class-and-struct
        public GameObject planet;

        public Light planetLight;
        public float mass;
        public float radius;
        public Vector3 velocity;
        public Vector3 acceleration;
        public Vector3 actualPosition;

    }

    // CSV data and JSON data use their own data types.
    public BodyProperty[] solarBodiesCSV;
    // public SolarBody[] solarBodiesJSON;

    // Both loader scripts finish reading their files in Awake().
    void Start()
    {
        playbutton.onClick.AddListener( () =>
        {
            playing = !playing;
            for (int i = 0; i < anglesliders.Length; i++)
            {
                anglesliders[i].interactable = !anglesliders[i].interactable;
            }
        });
        synth = GetComponent<PlanetSynth>();
        // CSV: use this block when a DataCSV component is attached.
        solarCSV = GetComponent<DataCSV>();
        Debug.Log(solarCSV == null);
        if (solarCSV != null)
        {
            solarBodiesCSV = solarCSV.bp;
            Debug.Log("Loaded " + solarBodiesCSV.Length + " bodies from solar.csv.");
            Debug.Log("First body: mass = " + solarBodiesCSV[0].mass + ", distance = " + solarBodiesCSV[0].distance + ", initial_velocity = " + solarBodiesCSV[0].initial_velocity);
        }

        Debug.Log(solarBodiesCSV.Length);
        // JSON: use this block when a DataJSON component is attached.
        // solarJSON = GetComponent<DataJSON>();
        // if (solarJSON != null)
        // {
        //     solarBodiesJSON = solarJSON.solarData.bodies;
        //     Debug.Log("Loaded " + solarBodiesJSON.Length + " bodies from solar.json.");
        //     Debug.Log("First body: " + solarBodiesJSON[0].name + ", mass: " + solarBodiesJSON[0].mass);
        // }


        // GameObject array to hold the planets in the simulation.
        planetProperties = new PlanetProperty[numberOfSphere];
        for (int i = 0; i < numberOfSphere; i++)
        {
            // Our gameobjects are created here:
            planetProperties[i] = new PlanetProperty();
            planetProperties[i].planet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            planetProperties[i].planetLight = planetProperties[i].planet.AddComponent<Light>();
            planetProperties[i].planetLight.type= LightType.Point;
        }

        // Apply the loaded data to the simulation. This is where you would set up your bodies in the scene based on the loaded data.
        for (int i = 0; i < solarBodiesCSV.Length; i++)
        {
            planetProperties[i].mass = solarBodiesCSV[i].mass;
            planetProperties[i].radius = solarBodiesCSV[i].radius;

            // What is missing here? You need to set the initial position and velocity of each planet based on the loaded data.
            // ***WRITE YOUR CODE HERE***
            float initAngle = 0f;//Random.Range(0.0f, 2f * Mathf.PI);
            float r = solarBodiesCSV[i].distance;
            planetProperties[i].actualPosition = new Vector3(r*Mathf.Cos(initAngle), r * Mathf.Sin(initAngle), 0f);
            planetProperties[i].velocity = 
              new Vector3(solarBodiesCSV[i].initial_velocity * Mathf.Cos(initAngle + Mathf.PI/2), 
              solarBodiesCSV[i].initial_velocity * Mathf.Sin(initAngle + Mathf.PI/2), 0f);

            // + This is just pretty trails
            TrailRenderer trailRenderer = planetProperties[i].planet.AddComponent<TrailRenderer>();
            // Configure the TrailRenderer's properties
            trailRenderer.time = 100.0f;  // Duration of the trail
            trailRenderer.startWidth = 0.5f;  // Width of the trail at the start
            trailRenderer.endWidth = 0.1f;    // Width of the trail at the end
            // a material to the trail
            trailRenderer.material = new Material(Shader.Find("Sprites/Default"));
            // Set the colour gradient along the trail.
            Gradient gradient = new Gradient();
            Color targetColor = Color.HSVToRGB((float)i / numberOfSphere, 1f, 1f);
            gradient.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(Color.white, 0f), // (color, normalized position)
                    new GradientColorKey(targetColor, 0.8f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1f, 0f), // (alpha, normalized position) 
                    new GradientAlphaKey(0f, 1f)
                }
            );
            trailRenderer.colorGradient = gradient;
        }
    }
    void FixedUpdate()
    {
        if (playing)
        {
            playingupdate();
        }
        else {setupupdate();}

    }


    private void setupupdate()
    {
        
        for(int i = 1; i < solarBodiesCSV.Length; i++)
        {
            float scaledDistance = Mathf.Sqrt(planetProperties[i].actualPosition.magnitude / 1e8f)/10.0f;
            float initAngle = anglesliders[i - 1].value;
            float r = solarBodiesCSV[i].distance;
            planetProperties[i].actualPosition = new Vector3(r*Mathf.Cos(initAngle), r * Mathf.Sin(initAngle), 0f);
            planetProperties[i].planet.transform.position = planetProperties[i].actualPosition.normalized * scaledDistance;

            planetProperties[i].velocity = 
              new Vector3(solarBodiesCSV[i].initial_velocity * Mathf.Cos(initAngle + Mathf.PI/2), 
              solarBodiesCSV[i].initial_velocity * Mathf.Sin(initAngle + Mathf.PI/2), 0f);
        }
    }

    private void playingupdate()
    {
        Debug.Log("playing");
        // Loop for N-body gravity
        // How should we design the loop?

        // 00. Initialize the acceleration for each body to zero at the start of each frame
       for (int i = 0; i < numberOfSphere; i++)
        {
            planetProperties[i].acceleration = Vector3.zero;
        }
        // 01. Loop through each body to calculate the gravitational forces acting on it
        for (int i = 0; i < numberOfSphere; i++)
        {
            
            for (int j = i + 1; j < numberOfSphere; j++)
            {
                Vector3 distance = planetProperties[i].actualPosition - planetProperties[j].actualPosition;
                Vector3 gravity = CalculateGravity(distance, planetProperties[i].mass, planetProperties[j].mass);

                planetProperties[i].acceleration -= gravity/planetProperties[i].mass;
                planetProperties[j].acceleration += gravity/planetProperties[j].mass;

            }
        }
        // 02. Loop through each body to update its velocity and position based on the calculated acceleration
       for (int i = 0; i < numberOfSphere; i++)
        {
            // ***WRITE YOUR CODE HERE***
            planetProperties[i].velocity += planetProperties[i].acceleration * Time.deltaTime * TimeController;
            planetProperties[i].actualPosition += planetProperties[i].velocity * Time.deltaTime * TimeController;

            float scaledDistance = Mathf.Sqrt(planetProperties[i].actualPosition.magnitude / 1e8f)/10.0f;
            planetProperties[i].planet.transform.position = planetProperties[i].actualPosition.normalized * scaledDistance;
            float x = planetProperties[i].planet.transform.position.x;
            float y = planetProperties[i].planet.transform.position.y;
            float theta = Mathf.Atan2(y, x);
            if (theta < playrange && theta > -playrange)
            {
                if(!synth.voices[i].held){
                synth.Enqueue(i, true, key[i].frequency);
                }
            }
            else
            {
                synth.Enqueue(i, false, 0f);
            }

        }
    }

    // Gravity Fuction to finish
    private Vector3 CalculateGravity(Vector3 distanceVector, float m1, float m2)
    {
        Vector3 gravity = Vector3.zero; // note this is also Vector3
        gravity = G * m1 * m2 / (distanceVector.sqrMagnitude) * distanceVector.normalized;
        return gravity;
    }
}
