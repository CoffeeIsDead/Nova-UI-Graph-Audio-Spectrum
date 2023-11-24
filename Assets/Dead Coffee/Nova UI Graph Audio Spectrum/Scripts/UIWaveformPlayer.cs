using Nova;
using UnityEngine;

public class UIWaveformPlayer : MonoBehaviour
{
    public int width = 1280;
    public int height = 64;
    public Color background = Color.black;
    public Color foreground = Color.yellow;
    [Header("Game Objects")]
    [SerializeField] UIBlock progressMarker;
    [SerializeField] UIBlock2D waveformUIBlock;
    [Header("Audio Source")]
    [SerializeField] AudioSource audioSource;
    private int samplesize;
    private float[] samples = null;
    private float[] waveform = null;
    private float audioSourceProgress = 0;

    private void Start()
    {

        Texture2D texwav = GetWaveform();
        waveformUIBlock.SetImage(texwav);
        waveformUIBlock.Size.XY.Value = new Vector2(width, height);

    }
    private void Update()
    {

        audioSourceProgress = (audioSource.time / audioSource.clip.length) * width;
        progressMarker.Position.X = audioSourceProgress;

    }
    private Texture2D GetWaveform()
    {

        int halfheight = height / 2;
        float heightscale = (float)height * 0.75f;

        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        waveform = new float[width];

        samplesize = audioSource.clip.samples * audioSource.clip.channels;
        samples = new float[samplesize];
        audioSource.clip.GetData(samples, 0);

        int packsize = (samplesize / width);
        for (int w = 0; w < width; w++)
        {

            waveform[w] = Mathf.Abs(samples[w * packsize])
                ;
        }

        for (int x = 0; x < width; x++)
        {

            for (int y = 0; y < height; y++)
            {

                tex.SetPixel(x, y, background);

            }
        }

        for (int x = 0; x < width; x++)
        {

            for (int y = 0; y < waveform[x] * heightscale; y++)
            {

                tex.SetPixel(x, halfheight + y, foreground);
                tex.SetPixel(x, halfheight - y, foreground);

            }
        }

        tex.Apply();

        return tex;

    }
}
