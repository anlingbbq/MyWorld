public class Block
{
    public string name;

    // 前后的贴图
    public int texture_x;
    public int texture_y;
    // 左右的贴图
    public int texture_x_lr;
    public int texture_y_lr;
    // 向上的贴图
    public int texture_x_top;
    public int texture_y_top;
    // 向下的贴图
    public int texture_x_bottom;
    public int texture_y_bottom;

    /// <summary>
    /// 前后左右一样，区分上下
    /// </summary>
    /// <param name="name"></param>
    /// <param name="tx"></param>
    /// <param name="ty"></param>
    /// <param name="ttx"></param>
    /// <param name="tty"></param>
    /// <param name="tbx"></param>
    /// <param name="tby"></param>
    public Block(string name, int tx, int ty, int ttx, int tty, int tbx, int tby)
    {
        this.name = name;
        texture_x = tx;
        texture_y = ty;
        texture_x_top = ttx;
        texture_x_bottom = tbx;
        texture_y_top = tty;
        texture_y_bottom = tby;
    }

    /// <summary>
    /// 所有面均不一样
    /// </summary>
    /// <param name="name"></param>
    /// <param name="tx"></param>
    /// <param name="ty"></param>
    /// <param name="tlx"></param>
    /// <param name="tly"></param>
    /// <param name="ttx"></param>
    /// <param name="tty"></param>
    /// <param name="tbx"></param>
    /// <param name="tby"></param>
    public Block(string name, int tx, int ty, int tlx, int tly, int ttx, int tty, int tbx, int tby)
    {
        this.name = name;
        texture_x = tx;
        texture_y = ty;
        texture_x_lr = tlx;
        texture_y_lr = tly;
        texture_x_top = ttx;
        texture_x_bottom = tbx;
        texture_y_top = tty;
        texture_y_bottom = tby;
    }
}
