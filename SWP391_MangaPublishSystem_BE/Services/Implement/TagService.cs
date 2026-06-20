using Entities.Models;
using Repositories.Repository;
using DTOs;
using Services.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Implement
{
    public class TagService : ITagService
    {
        private readonly TagRepository _tagRepository;

        public TagService(TagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<List<TagDto>> GetAllAsync()
        {
            var tags = await _tagRepository.GetAllAsync();
            return tags.Select(t => new TagDto
            {
                Tagid = t.Tagid,
                Tagname = t.Tagname
            }).ToList();
        }

        public async Task<TagDto?> GetByIdAsync(int id)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag == null) return null;

            return new TagDto
            {
                Tagid = tag.Tagid,
                Tagname = tag.Tagname
            };
        }

        public async Task<int> CreateAsync(TagDto.Create createDto)
        {
            var tag = new Tag
            {
                Tagname = createDto.Tagname
            };

            await _tagRepository.CreateAsync(tag);
            return tag.Tagid;
        }

        public async Task<bool> UpdateAsync(int id, TagDto.Update updateDto)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag == null) return false;

            tag.Tagname = updateDto.Tagname;

            return await _tagRepository.UpdateAsync(tag) > 0;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag == null) return false;

            return await _tagRepository.RemoveAsync(tag);
        }
    }
}
