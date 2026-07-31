# 资源共享

一款资源合集分享工具，支持 PC 端和 Android 端。

## 功能特性

- 17 种主题背景切换（深空、极光、霓虹、暗红、海洋、森林、日落、月光、赛博、暗金、薰衣草、薄荷、午夜、珊瑚、冰蓝、琥珀、玫瑰）
- QQ 交流频道和微信公众号快捷入口
- 夸克网盘、迅雷网盘资源分类管理
- 版本更新弹窗提示
- 粒子动画和扫描线特效
- 响应式设计，适配 PC 和手机
- 支持全面屏设备安全区域适配

## 技术栈

| 平台 | 技术 |
|------|------|
| 前端 | HTML5 + CSS3 + JavaScript |
| PC端 | Electron |
| Android | Capacitor |
| 安装程序 | C# WinForms |

## 项目结构

`
├── index.html              # 主页面（单文件应用）
├── main.js                 # Electron 主进程
├── package.json            # 项目配置
├── capacitor.config.json   # Capacitor 配置
├── installer_gui.cs        # PC 安装程序源码
├── uninstaller.cs          # PC 卸载程序源码
├── build/                  # 图标资源
├── android/                # Android 项目
└── www/                    # Web 资源目录
`

## 快速开始

### PC 端开发
`ash
npm install
npm start
`

### Android 开发
`ash
npm run cap-sync
cd android && gradlew assembleDebug
`

### 构建安装包
`ash
npm run build-win
`

## 下载

- [PC 安装包](../../releases) - Windows 安装程序
- [Android APK](../../releases) - Android 安装包

## 版权信息

Copyright (c) 2026 Ajian920

本项目基于 MIT 许可证开源，详见 [LICENSE](LICENSE)。

## 联系方式

- 邮箱：Android_995@163.com
- QQ 交流频道：[加入](https://qr61.cn/oTZBZj/qvpZXKn)
- 微信公众号：[关注](https://qr61.cn/oTZBZj/q7grPss)