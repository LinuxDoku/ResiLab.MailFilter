namespace ResiLab.MailFilter.Configuration {
    public enum RuleType {
        SenderEquals,
        SenderContains,
        SenderEndsWith,

        SenderNameEquals,
        SenderNameContains,
        SenderNameBeginsWith,
        SenderNameEndsWith,

        SubjectEquals,
        SubjectContains,
        SubjectBeginsWith,
        SubjectEndsWith
    }
}