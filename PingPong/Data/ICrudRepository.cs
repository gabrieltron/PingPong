namespace PingPong.Data
{
    public interface ICrudRepository<TModel, TIdType>
    {
        public Task<IEnumerable<TModel>> FindAll();

        public Task Create(TModel model);

        public Task<TModel> FindOne(TIdType id);

        public Task Update(TModel model);

        public Task Delete(TIdType id);
    }
}
