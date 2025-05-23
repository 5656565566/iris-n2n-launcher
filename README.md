## 介绍
本项目是一个使用 .NET 8 开发的 Winform
用于启动 N2N 的 edge 节点 并提供一些额外的功能

如：

1. 广播修复（用于修复 Windows 中只有默认网卡能发送广播的问题）
2. 端口映射
3. Minecraft 房间搜索
4. 简易的文件发送

等功能

项目使用 MIT 许可证进行开源，请注意相关条款
项目中的所有 图像资源，不在 MIT 许可范围内。

## 📦 已集成组件

本项目中包含以下项目或分支**原样编译或构建**的可执行程序，均未修改其源码或进行静态链接，且符合其许可证要求：

| 组件介绍 | 来源仓库 | 可执行文件 | 许可证 | 原仓库 (如果有) |
|-|-|-|-|-|
| 虚拟局域网边缘节点 | [n2n](https://github.com/5656565566/n2n) | `edge.exe` | GPLv3 | https://github.com/ntop/n2n |
| 局域网 IP 广播转发器 | [WinIPBroadcast](https://github.com/5656565566/WinIPBroadcast) | `WinIPBroadcast.exe` | GPLv3 | https://github.com/dechamps/WinIPBroadcast |
| TCPUDP 端口转发器 | [orchid-tcpudp-forw](https://github.com/5656565566/orchid-tcpudp-forw) | `orchid-tcpudp-forw.exe` | MIT | 来源仓库 略 |
| 虚拟网卡驱动 | [tap-windows6](https://github.com/OpenVPN/tap-windows6) | `略` | GPLv2 + Microsoft WDK 例外 | 来源仓库 略 |

以上组件均未放置在本仓库，如需二次开发，可以补全组件，请遵守原仓库的协议使用
