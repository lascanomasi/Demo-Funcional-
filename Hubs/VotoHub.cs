using Microsoft.AspNetCore.SignalR;

namespace VotaYa.Hubs
{
    // Este Hub es lo que hace la magia del tiempo real
    // Cuando alguien vota, SignalR avisa a TODOS los clientes conectados
    public class VotoHub : Hub
    {
        // El servidor llama a este método para actualizar resultados en todas las pantallas
        public async Task ActualizarResultados(int opcionId, int totalVotos)
        {
            await Clients.All.SendAsync("ResultadoActualizado", opcionId, totalVotos);
        }
    }
}
