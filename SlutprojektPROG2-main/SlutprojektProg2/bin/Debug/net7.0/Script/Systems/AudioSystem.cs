
public class AudioSystem : GameSystem
{
    public override void Update()
    {
        foreach (Entity e in Game.entities)
        {
            AudioPlayer? audioPlayer = e.GetComponent<AudioPlayer>();

            if (audioPlayer != null)
            {
                if (Raylib.IsSoundReady(audioPlayer.audioClip) && !Raylib.IsSoundPlaying(audioPlayer.audioClip))
                    Raylib.PlaySound(audioPlayer.audioClip);
            }
        }
    }

    private float VolumeFalloff(Entity entity1, Entity entity2)
    {
        return Raymath.Vector2Distance(entity1.position, entity2.position);
    }
}