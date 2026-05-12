namespace QuanLyKhoBanHang.DAL.Data;

public abstract class RepositoryBase
{
    protected RepositoryBase(DatabaseOptions options)
    {
        Options = options;
    }

    protected DatabaseOptions Options { get; }
}
