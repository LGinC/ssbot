# ssbot
通过[go-cqhttp](https://github.com/Mrs4s/go-cqhttp)接收qq机器人发来的消息，并自动调用go-cqhttp api发送消息等

比如做一个涩涩机器人, 发送以!!或者！！开头的群消息，机器人就会开始进行解析，包含涩图则会发一张涩图出来
![image](https://user-images.githubusercontent.com/15363011/192490956-1ea999f9-78e4-4603-b402-73c7cb181ac5.png)


# 部署方式
`docker run -d --name=ssbot -p 7500:80 -v $PWD/setu:/app/images/setu -e Cqhttp__Host=http://192.168.1.100:6700 -e Cqhttp__secret=myqq lginc/ssbot`

其中`-p 7500:80` 是把容器的80端口映射到宿主机的7500端口，请自行替换

`-v $PWD/setu:/app/images/setu` 是把宿主机当前目录下的setu目录映射到容器的/app/images/setu， 可以把$PWD/setu替换为你自己的涩图所在的目录

环境变量Cqhttp__Host是go-cqhttp服务的url，Cqhttp__secret是发送消息所需要的access_token，请自行替换

# 修改cqhttp配置

## 修改post上传url
在[cqhttp配置](https://docs.go-cqhttp.org/guide/config.html#%E9%85%8D%E7%BD%AE%E4%BF%A1%E6%81%AF)中有关于post url的配置
![image](https://user-images.githubusercontent.com/15363011/192493254-fba159a7-f721-47d3-bc5f-4e5fef3d706d.png)

这里取消注释(删除行首的#)，  url填入ssbot服务的url，如宿主机是192.168.1.100，端口也是7500，则填写为
```yaml
post:           # 反向HTTP POST地址列表
  - url: 'http://192.168.1.100:7500/recv'                # 地址
```
## 修改默认的filter
filter.json里增加对！！的过滤  \uFF01就是！的unicode
```json
{
    "raw_message": {
        ".regex": "^(!!|\uFF01\uFF01)"
    }
}
```

然后重启cqhttp服务，就可以愉快地涩涩了
