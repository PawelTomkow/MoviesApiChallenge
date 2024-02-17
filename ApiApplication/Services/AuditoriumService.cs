using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApiApplication.Core.Exceptions;
using ApiApplication.Core.Models;
using ApiApplication.Core.Services;
using ApiApplication.Database.Repositories.Abstractions;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace ApiApplication.Services
{
    public class AuditoriumService : IAuditoriumService
    {
        private readonly IAuditoriumsRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<AuditoriumService> _logger;

        public AuditoriumService(IAuditoriumsRepository repository, IMapper mapper, ILogger<AuditoriumService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }
        
        public async Task<List<Auditorium>> GetAllAsync(CancellationToken cancellationToken, bool withSeats = false)
        {
            var entities = await _repository.GetAllAsync(cancellationToken, withSeats);
            var domainObject = _mapper.Map<List<Auditorium>>(entities);
            return domainObject;
        }

        public async Task<Auditorium> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetAsync(id, cancellationToken);
            if (entity is null)
            {
                _logger.LogInformation("Auditorium with id: {id} not found.", id);
                throw new ResourceNotFoundException(typeof(Auditorium), nameof(id), id.ToString());
            }
            var domainObject = _mapper.Map<Auditorium>(entity);
            return domainObject;
        }
    }
}