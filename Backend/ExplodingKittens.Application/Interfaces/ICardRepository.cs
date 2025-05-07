using System.Collections.Generic;
using System.Threading.Tasks;
using ExplodingKittens.Domain.Entities;
using ExplodingKittens.Domain.Enums;

namespace ExplodingKittens.Application.Interfaces
{
    /// <summary>
    /// Repository interface for Card-specific operations
    /// </summary>
    public interface ICardRepository : IRepository<Card>
    {
        Task<IEnumerable<Card>> GetCardsByTypeAsync(CardType type);
        Task<IEnumerable<Card>> GetCardsByIdsAsync(IEnumerable<string> ids);
        Task<Card> GetRandomCardAsync();
    }
}