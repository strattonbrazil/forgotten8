using System;
namespace forgotten.Desktop
{
    public class DialogueStateMachine
    {
        private DialogueState currentState;
        public DialogueStateMachine(DialogueState initState)
        {
            currentState = initState;
        }

        public string Text()
        {
            return currentState.Text;
        }

        public void SelectionOption(int index)
        {
            currentState = currentState.Options[index].Outcome(currentState);
        }

        public DialogueOption[] Options()
        {
            return currentState.Options;
        }

        // consider not exposing this to avoid relying directly on it
        public DialogueState State()
        {
            return currentState;
        }
    }

    public abstract class DialogueState
    {
        public string Text;
        public DialogueOption[] Options;
    }

    public class AlienEncounter : DialogueState
    {
        public AlienEncounter() {
            Text = @"""Halt, Terran!""
""You have ventured into *blurp* Korgat space and are under our *blurp* jurisdication *blurp*.""

""State your business or be escorted back to united space.""";
            Options = new DialogueOption[] {
                new DialogueOption()
                {
                    Text = @"""[Bluff] I was on my way out.""",
                    Outcome = delegate (DialogueState prevState)
                    {
                        return new LiesAngerEncounter();
                    }
                },
                new DialogueOption()
                {
                    Text = @"""I'm here to assassinate your Premiere.""",
                    Outcome = delegate (DialogueState prevState)
                    {
                        return null;
                    }
                }
            };
        }
    }

    public class LiesAngerEncounter : DialogueState
    {
        public LiesAngerEncounter()
        {
            Text = @"""Lies!""";
            Options = new DialogueOption[] {
                new DialogueOption()
                {
                    Text = @" (enter combat) ",
                    Outcome = delegate (DialogueState prevState)
                    {
                        return null;
                    }
                }
            };
        }
    }


    public delegate DialogueState OutcomeDelegate(DialogueState prevState);

    public class DialogueOption
    {
        public string Text;
        public OutcomeDelegate Outcome;
    }
}
