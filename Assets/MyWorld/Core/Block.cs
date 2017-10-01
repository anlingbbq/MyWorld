public class Block
{
    public string name;

    // 前后的贴图
    public int texture_u_fb;
    public int texture_v_fb;
    // 左右的贴图
    public int texture_u_lr;
    public int texture_v_lr;
    // 向上的贴图
    public int texture_u_top;
    public int texture_v_top;
    // 向下的贴图
    public int texture_u_bottom;
    public int texture_v_bottom;

    /// <summary>
    /// 前后左右一样，区分上下
    /// </summary>
    /// <param name="name"></param>
    /// <param name="tru">四周</param>
    /// <param name="trv">四周</param>
    /// <param name="ttu">上</param>
    /// <param name="ttv">上</param>
    /// <param name="tbu">下</param>
    /// <param name="tbv">下</param>
    public Block(string name, int tru, int trv, int ttu, int ttv, int tbu, int tbv)
    {
        this.name = name;
        texture_u_fb = tru;
        texture_v_fb = trv;
        texture_u_lr = tru;
        texture_v_lr = trv;
        texture_u_top = ttu;
        texture_v_top = ttv;
        texture_u_bottom = tbu;
        texture_v_bottom = tbv;
    }

    /// <summary>
    /// 所有面均不一样
    /// </summary>
    /// <param name="name"></param>
    /// <param name="tfu">前后</param>
    /// <param name="tfv">前后</param>
    /// <param name="tlu">左右</param>
    /// <param name="tlv">左右</param>
    /// <param name="ttu">上</param>
    /// <param name="ttv">上</param>
    /// <param name="tbu">下</param>
    /// <param name="tbv">下</param>
    public Block(string name, int tfu, int tfv, int tlu, int tlv, int ttu, int ttv, int tbu, int tbv)
    {
        this.name = name;
        texture_u_fb = tfu;
        texture_v_fb = tfv;
        texture_u_lr = tlu;
        texture_v_lr = tlv;
        texture_u_top = ttu;
        texture_v_top = ttv;
        texture_u_bottom = tbu;
        texture_v_bottom = tbv;
    }
}
