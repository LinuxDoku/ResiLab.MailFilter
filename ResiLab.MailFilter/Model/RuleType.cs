namespace ResiLab.MailFilter.Model {
    public enum RuleType {
        SenderEquals,
        SenderContains,
        SenderEndsWith,

        SubjectEquals,
        SubjectContains,
        SubjectBeginsWith,
        SubjectEndsWith
    }
}