namespace Domain.Models.Genesys;

public record AuthToken(
	string? AccessToken = null,
	string? RefreshToken = null,
	string? TokenType = null,
	int? ExpiresIn = null,
	string? Error = null);