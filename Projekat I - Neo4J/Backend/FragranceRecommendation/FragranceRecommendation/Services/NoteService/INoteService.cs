using FragranceRecommendation.DTOs.NoteDTOs;

namespace FragranceRecommendation.Services.NoteService;

public interface INoteService
{
    public Task<bool> NoteExistsAsync(string name);
    public Task<IList<INode>> GetNotesAsync();
    public Task<Note> GetNoteAsync(string name);
    public Task AddNoteAsync(AddNoteDto note);
    public Task UpdateNoteAsync(UpdateNoteDto note);
    public Task DeleteNoteAsync(DeleteNoteDto note);
}