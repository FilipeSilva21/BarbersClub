namespace Business.Error_Handling;

public class UserIdNotFoundException(int userId) : Exception($"Usuário com ID {userId} não foi encontrado.");

public class EmailNotFoundException(string email) : Exception($"Usuário com email {email} não foi encontrado.");

public class ServiceNotFoundException(int serviceId) : Exception($"Serviço com ID {serviceId} não foi encontrado.");

public class InvalidCredentialsException(string invalidPassword) : Exception($"Usuario ou senha incorretos.");

public class BarberShopNotFoundException(int barberShopId) : Exception($"Barbearia com ID {barberShopId} não foi encontrada.");

public class RatingNotFoundException(int ratingId) : Exception($"Avaliação com ID {ratingId} não foi encontrada.");

public class DuplicateEmailException(string email) : Exception($"Já existe um usuário com o email {email}.");

public class ServiceAlreadyScheduledException(DateTime dateTime) : Exception($"Já existe um serviço agendado para {dateTime}.");

public class RatingAlreadyExistException(int serviceId) : Exception($"Já existe uma avaliação para o servico com ID {serviceId}.");