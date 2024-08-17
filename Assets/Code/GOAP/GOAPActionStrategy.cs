namespace Curly.GOAP
{
    public interface IGOAPActionStrategy
    {
        bool CanPerform { get; }
        bool Complete { get; }

        void Start() {
            // noop
        }

        void Update(float deltaTime) {
            // noop
        }

        void Finish() {
            // noop
        }
    }
}
