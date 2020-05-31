using System;
namespace forgotten.Desktop
{
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

                    }
                },
                new DialogueOption()
                {
                    Text = @"""I'm here to assassinate your Premiere.""",
                    Outcome = delegate (DialogueState prevState)
                    {


                    }
                }
            };
        }
    }

    public delegate void OutcomeDelegate(DialogueState prevState);

    public class DialogueOption
    {
        public string Text;
        public OutcomeDelegate Outcome;
    }
}
