namespace FragranceRecommendation.Services.NoteService;

public interface INoteService
{
    public Task<bool> NoteExistsAsync(string name);
    public Task<IList<Note>> GetNotesAsync();
    public Task<Note?> GetNoteAsync(string name);
    public Task AddNoteAsync(AddNoteDto note);
    public Task UpdateNoteAsync(UpdateNoteDto note);
    public Task DeleteNoteAsync(string name);
}