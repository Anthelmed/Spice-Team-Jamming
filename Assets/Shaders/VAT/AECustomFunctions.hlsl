void MatrixInverse_float(float4x4 input, out float4x4 result)
{
    float det;
    float4x4 inv;

    inv[0][0] = input[1][1] * input[2][2] * input[3][3] -
        input[1][1] * input[2][3] * input[3][2] -
        input[2][1] * input[1][2] * input[3][3] +
        input[2][1] * input[1][3] * input[3][2] +
        input[3][1] * input[1][2] * input[2][3] -
        input[3][1] * input[1][3] * input[2][2];

    inv[1][0] = -input[1][0] * input[2][2] * input[3][3] +
        input[1][0] * input[2][3] * input[3][2] +
        input[2][0] * input[1][2] * input[3][3] -
        input[2][0] * input[1][3] * input[3][2] -
        input[3][0] * input[1][2] * input[2][3] +
        input[3][0] * input[1][3] * input[2][2];

    inv[2][0] = input[1][0] * input[2][1] * input[3][3] -
        input[1][0] * input[2][3] * input[3][1] -
        input[2][0] * input[1][1] * input[3][3] +
        input[2][0] * input[1][3] * input[3][1] +
        input[3][0] * input[1][1] * input[2][3] -
        input[3][0] * input[1][3] * input[2][1];

    inv[3][0] = -input[1][0] * input[2][1] * input[3][2] +
        input[1][0] * input[2][2] * input[3][1] +
        input[2][0] * input[1][1] * input[3][2] -
        input[2][0] * input[1][2] * input[3][1] -
        input[3][0] * input[1][1] * input[2][2] +
        input[3][0] * input[1][2] * input[2][1];

    det = input[0][0] * inv[0][0] + input[0][1] * inv[1][0] + input[0][2] * inv[2][0] + input[0][3] * inv[3][0];

    if (det == 0)
        result = float4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
    else
    {
        det = 1.0 / det;

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                inv[i][j] *= det;
            }
        }

        result = inv;
    }
}