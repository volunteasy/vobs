namespace Volunteasy.Core.Errors;

public class InvalidPasswordException : ApplicationException { }
public class DuplicateEmailException : ApplicationException { }

public class EmailNotFoundException : ApplicationException { }

public class UnauthenticatedUserException : ApplicationException { }