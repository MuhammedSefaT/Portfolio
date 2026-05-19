using Portfolio.Common.Dtos;
using Portfolio.Entity;

namespace Portfolio.Business.Interface;

public interface ICertificateService : IGenericService<Certificate, CertificateListDto, CertificateCreateDto, CertificateUpdateDto>
{
}
