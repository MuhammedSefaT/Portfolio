using AutoMapper;
using FluentValidation;
using Portfolio.Business.Interface;
using Portfolio.Common.Dtos;
using Portfolio.DataAccess.Intrerface;
using Portfolio.Entity;

namespace Portfolio.Business.Service;

public class CertificateService : GenericService<Certificate, CertificateListDto, CertificateCreateDto, CertificateUpdateDto>, ICertificateService
{
    public CertificateService(IUow uow, IGenericRepository<Certificate> repository, IMapper mapper, IValidator<CertificateCreateDto> createValidator, IValidator<CertificateUpdateDto> updateValidator) : base(uow, repository, mapper, createValidator, updateValidator)
    {
    }
}
