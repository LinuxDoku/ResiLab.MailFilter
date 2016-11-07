namespace ResiLab.MailFilter.Configuration {
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