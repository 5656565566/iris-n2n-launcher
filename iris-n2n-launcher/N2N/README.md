## 使用的已编译第三方程序

本项目包含以下从第三方项目或其分支编译的可执行文件，均遵循原项目的开源许可证：

| 功能描述           | 项目地址                                                                 | 可执行文件           | 备注说明           |
|--------------------|--------------------------------------------------------------------------|----------------------|--------------------|
| TCP/UDP 端口转发器 | [orchid-tcpudp-forw](https://github.com/5656565566/orchid-tcpudp-forw)   | `orchid-tcpudp-forw.exe` | MIT 许可证         |
| 局域网 VPN 节点    | [n2n](https://github.com/5656565566/n2n)                                 | `edge.exe`           | GPLv3，改自 ntop/n2n |
| 局域网 IP 广播桥接 | [WinIPBroadcast](https://github.com/5656565566/WinIPBroadcast)           | `WinIPBroadcast.exe` | GPLv3，改自 dechamps/WinIPBroadcast  |

>  以上可执行程序均来自其他开源项目的构建，未对其源代码进行修改或静态链接，仅用于独立功能调用，此处不包含这些文件，请自行前往相关仓库获取它们。
