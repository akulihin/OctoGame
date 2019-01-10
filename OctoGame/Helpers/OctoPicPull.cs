using System.Threading.Tasks;

namespace OctoGame.Helpers
{


    public sealed  class OctoPicPull : IService
    {
        public Task InitializeAsync()
            => Task.CompletedTask;
        public string[] OctoPics =
        {
            //индекс осьминога -1 к его номеру
            "https://i.imgur.com/NQlAjwN.png", //1 розовый книга
            "https://i.imgur.com/rwQb4Zz.jpg", //2 Сервый верхтормашками
            "https://i.imgur.com/PoMF5Pn.jpg", //3
            "https://i.imgur.com/JhKwGgq.jpg", //4
            "https://i.imgur.com/puNz7pu.jpg", //5 черный с ножом
            "https://i.imgur.com/C44tEp4.jpg", //6
            "https://i.imgur.com/I5SoZU5.jpg", //7
            "https://i.imgur.com/KjbRCLE.jpg", //8
            "https://i.imgur.com/dxJq2Ey.png", //9
            "https://i.imgur.com/0Q2qoFh.jpg", //10 Жёлтый рисует
            "https://i.imgur.com/U4BCc4q.jpg", //11
            "https://i.imgur.com/chSMjPQ.jpg", //12
            "https://i.imgur.com/aqKDTB0.jpg", //13
            "https://i.imgur.com/axBcQrv.jpg", //14
            "https://i.imgur.com/pjaroXG.jpg", //15
            "https://i.imgur.com/BlqRoJX.jpg", //16
            "https://i.imgur.com/cIO2Nm7.jpg", //17
            "https://i.imgur.com/DTqlQ7J.jpg", //18
            "https://i.imgur.com/VGHWFwj.jpg", //19
            "https://i.imgur.com/odNQZqA.jpg", //20 Жёлтый с геймпадом
            "https://i.imgur.com/Hrm3FoB.jpg", //21
            "https://i.imgur.com/dM1fylv.jpg", //22
            "https://i.imgur.com/h3oUUsL.jpg", //23
            "https://i.imgur.com/EyxWjOl.jpg", //24
            "https://i.imgur.com/N1yX1p9.jpg", //25
            "https://i.imgur.com/D3CDBxh.jpg", //26
            "https://i.imgur.com/Xkv4kjs.jpg", //27 Жёлты Нвоый год
            "https://i.imgur.com/UZGqdub.jpg", //28
            "https://i.imgur.com/9Zh2wlg.jpg", //29
            "https://i.imgur.com/fzCQxDp.jpg", //30
            "https://i.imgur.com/wBAuR1c.jpg", //31
            "https://i.imgur.com/42vQjK1.jpg", //32
            "https://i.imgur.com/RD6hXRA.jpg", //33
            "https://i.imgur.com/EL4tj6W.jpg", //34
            "https://i.imgur.com/2czyupr.jpg", //35
            "https://i.imgur.com/f3Jlmo5.jpg", //36
            "https://i.imgur.com/YfsWeVw.jpg", //37
            "https://i.imgur.com/H1VpUXU.jpg", //38
            "https://i.imgur.com/39F8TIt.jpg", //39
            "https://i.imgur.com/TDmVtkU.jpg", //40
            "https://i.imgur.com/B018LtR.jpg", //41
            "https://i.imgur.com/etRphwD.jpg", //42
            "https://i.imgur.com/rjaNOYB.jpg", //43
            "https://i.imgur.com/y0AeviU.jpg", //44 Davlas Octo
            "https://i.imgur.com/47dNg8Y.jpg", //45
            "https://i.imgur.com/HcsXFSr.jpg", //46 OCto-spider
            "https://i.imgur.com/KfmI8d3.jpg", //47
            "https://i.imgur.com/AlTg8Gi.jpg", //48 (-1)
            "https://i.imgur.com/k9y6RmI.jpg", //49 ДРЯКОООНЬ!
            "https://i.imgur.com/h1lVuBH.jpg", //50 (-1 alsways!)
            "https://i.imgur.com/I9xLycY.jpg", //51 пипашка в шапке!
            "https://i.imgur.com/OJvPxlQ.jpg", //52 дряконы!
            "https://i.imgur.com/0PGcMv5.jpg", //53
            "https://i.imgur.com/Mpnj6ry.jpg", //54
            "https://i.imgur.com/3T66o9I.jpg", //55
            "https://i.imgur.com/VNphTYI.jpg", //56 Pachi with headphones
            "https://i.imgur.com/PSi8N2i.jpg", //57 2Rainbow and green Boo
            "https://i.imgur.com/5gh7Kgs.jpg", //58 OctoSpider + Purple-Standing
            "https://i.imgur.com/akfktDb.jpg", //59  Rainbow on YellowTurtle
            "https://i.imgur.com/Pn9Sgl0.jpg", //60 Rainbow LeCrisp
            "https://i.imgur.com/YiW43ad.jpg", //61 GreenBoo With a knife
            "https://i.imgur.com/yMSGQYu.jpg", //62 Братишки
            "https://i.imgur.com/GCKywPO.jpg", //63 братишка за компом
            "https://i.imgur.com/i7CsKuk.jpg", //64 братишка и много осьминогов
            "https://i.imgur.com/CVkzrjJ.jpg",
            "https://i.imgur.com/bW57jhq.jpg",
            "https://i.imgur.com/mJxMuLu.jpg",
            "https://i.imgur.com/4EPD5kc.jpg",
            "https://i.imgur.com/Es7Lo8Z.jpg",
            "https://i.imgur.com/BqV2Oey.jpg",
            "https://i.imgur.com/WN3T2yE.jpg",
            "https://i.imgur.com/tQQ2Xpy.jpg",
            "https://i.imgur.com/kxT0aEj.jpg",
            "https://i.imgur.com/5caqVQg.jpg",
            "https://i.imgur.com/LB6oLLQ.jpg",
            "https://i.imgur.com/LpqeU77.jpg",
            "https://i.imgur.com/r85Dn8t.jpg",
            "https://i.imgur.com/Kr3umkC.jpg",
            "https://i.imgur.com/kB1vV39.jpg",
            "https://i.imgur.com/IGLBtax.jpg",
            "https://i.imgur.com/GXIKFvR.jpg",
            "https://i.imgur.com/rG8gwaZ.jpg",
            "https://i.imgur.com/wo0lJcw.jpg",
            "https://i.imgur.com/rv5YnTm.jpg",
            "https://i.imgur.com/4anWFFb.jpg",
            "https://i.imgur.com/I6eAl2d.jpg",
            "https://i.imgur.com/3i22uVz.jpg",
            "https://i.imgur.com/eMTolZz.jpg",
            "https://i.imgur.com/fxjxQCh.jpg",
            "https://i.imgur.com/QK6hZ5B.jpg",
            "https://i.imgur.com/iQ7XFAF.jpg",
            "https://i.imgur.com/wG8q6E1.jpg",
            "https://i.imgur.com/IJCrm1b.jpg",
            "https://i.imgur.com/bAzj6zr.jpg",
            "https://i.imgur.com/LtRVQDq.jpg",
            "https://i.imgur.com/OpOhpvs.jpg",
            "https://i.imgur.com/N4Wyye0.jpg",
            "https://i.imgur.com/qQCpt6g.jpg",
            "https://i.imgur.com/i8MjyRR.jpg",
            "https://i.imgur.com/baQMObw.jpg",
            "https://i.imgur.com/DavR8C7.jpg",
            "https://i.imgur.com/JyI8PNU.jpg",
            "https://i.imgur.com/ZtrESXP.jpg",
            "https://i.imgur.com/OTcCJwX.jpg",
            "https://i.imgur.com/OXoFjjr.jpg",
            "https://i.imgur.com/wiskanY.jpg",
            "https://i.imgur.com/fO8YZGA.jpg",
            "https://i.imgur.com/q9D4TVK.png",
            "https://i.imgur.com/kdc2b9z.jpg",
            "https://i.imgur.com/xAYL5vM.jpg",
            "https://i.imgur.com/VLJNGRG.jpg",
            "https://i.imgur.com/cTqk5ci.jpg",
            "https://i.imgur.com/vxTe2sM.jpg",
            "https://i.imgur.com/sn9qfSb.jpg",
            "https://i.imgur.com/5hmem4s.jpg",
            "https://i.imgur.com/gmsOGAF.jpg",
            "https://i.imgur.com/6CiWZjK.jpg",
            "https://i.imgur.com/ZWSwSDS.jpg",
            "https://i.imgur.com/NyTHBeV.jpg",
            "https://i.imgur.com/dqppxFf.jpg",
            "https://i.imgur.com/4MykFJm.jpg",
            "https://i.imgur.com/RoqjD7C.jpg",
            "https://i.imgur.com/eHQEjjK.jpg",
            "https://i.imgur.com/231MgpJ.jpg",
            "https://cdn.discordapp.com/attachments/425865177813090314/445366377067577354/JPEG_20180218_032048.jpg",
            "https://cdn.discordapp.com/attachments/425865177813090314/445366374731218944/JPEG_20180201_211154.jpg",
            "https://cdn.discordapp.com/attachments/425865177813090314/445366379869241344/JPEG_20180223_120340.jpg",
            "https://cdn.discordapp.com/attachments/425865177813090314/445366384558604290/JPEG_20180410_083052.jpg",
            "https://cdn.discordapp.com/attachments/425865177813090314/445366382939471884/JPEG_20180314_221039.jpg",
            "https://cdn.discordapp.com/attachments/425865177813090314/445366388597587968/JPEG_20171005_002230.jpg",
            "https://cdn.discordapp.com/attachments/425865177813090314/445366386110365697/JPEG_20180413_004513.jpg",
            "https://cdn.discordapp.com/attachments/425865177813090314/445366387536560128/JPEG_20171005_001804.jpg",
            "https://cdn.discordapp.com/attachments/425865177813090314/445366606575435777/JPEG_20171006_173629.jpg",
            "https://cdn.discordapp.com/attachments/425865177813090314/445366619842019339/JPEG_20171014_075205.jpg",
            "https://cdn.discordapp.com/attachments/425865177813090314/445366639274491904/JPEG_20180124_141420.jpg",
            "https://cdn.discordapp.com/attachments/425865177813090314/445366662880034839/JPEG_20180128_211126.jpg",
            "https://cdn.discordapp.com/attachments/425865177813090314/445366385934204929/Screenshot_44.png",
            "https://media.discordapp.net/attachments/370950995133464587/442770494937104395/IMG_20180506_122658.jpg?width=936&height=702",
            "https://media.discordapp.net/attachments/370950995133464587/442770494089986064/IMG_20180506_122840.jpg?width=936&height=702",
            "https://media.discordapp.net/attachments/370950995133464587/442770494089986058/IMG_20180505_115422.jpg?width=936&height=702",
            "https://media.discordapp.net/attachments/370950995133464587/442770493599383562/IMG_20180506_112852.jpg?width=936&height=702",
            "https://media.discordapp.net/attachments/370950995133464587/442770492798009355/IMG_20180505_115640.jpg?width=936&height=702",
            "https://media.discordapp.net/attachments/370950995133464587/445467032176295947/IMG_20180506_121914.jpg?width=1872&height=1404"
        };

        public string[] OctoPicsPull =
        {
            "https://cdn.discordapp.com/attachments/436071383836000256/467125305535102976/20180712_201641.jpg",
            "https://cdn.discordapp.com/attachments/436071383836000256/467125306331889664/20180712_201653.jpg",
            "https://cdn.discordapp.com/attachments/436071383836000256/467125306331889665/20180712_201701.jpg",
            "https://cdn.discordapp.com/attachments/436071383836000256/467125272328536094/20180712_201710.jpg",
            "https://cdn.discordapp.com/attachments/436071383836000256/467125271963762690/20180712_201729.jpg", //5
            "https://cdn.discordapp.com/attachments/436071383836000256/467125271963762688/20180712_201744.jpg",
            "https://cdn.discordapp.com/attachments/436071383836000256/467125221225398285/20180712_201754.jpg",
            "https://cdn.discordapp.com/attachments/436071383836000256/467125221225398283/20180712_201806.jpg",
            "https://cdn.discordapp.com/attachments/436071383836000256/467125220759568384/20180712_201821.jpg",
            "https://cdn.discordapp.com/attachments/436071383836000256/467125122378235904/20180712_201839.jpg", //10
            "https://cdn.discordapp.com/attachments/436071383836000256/467125122189361173/20180712_201849.jpg",
            "https://cdn.discordapp.com/attachments/436071383836000256/467125121920794624/20180712_201857.jpg",
            "https://cdn.discordapp.com/attachments/436071383836000256/467125088999702529/20180712_201926.jpg",
            "https://cdn.discordapp.com/attachments/436071383836000256/467125087884279820/20180712_201947.jpg",
            "https://cdn.discordapp.com/attachments/436071383836000256/467125089431846922/20180712_201953.jpg", //15
            "https://cdn.discordapp.com/attachments/436071383836000256/467125051804614685/20180712_202005.jpg",
            "https://cdn.discordapp.com/attachments/436071383836000256/467125052064792577/20180712_202026.jpg",
            "https://cdn.discordapp.com/attachments/436071383836000256/467125052064792576/20180712_202037.jpg",
            "https://cdn.discordapp.com/attachments/436071383836000256/467124945617682432/20180712_202045.jpg",
            "https://cdn.discordapp.com/attachments/436071383836000256/467125992910225418/20180712_203149.jpg", // 20
            "https://cdn.discordapp.com/attachments/436071383836000256/467124944749199360/20180712_202059.jpg",
            "https://media.discordapp.net/attachments/436071383836000256/467129243663466516/20180712_204433.jpg",
            "https://cdn.discordapp.com/attachments/436071383836000256/467124913254170627/20180712_202111.jpg",
            "https://cdn.discordapp.com/attachments/436071383836000256/467124913254170625/20180712_202139.jpg",
            "https://cdn.discordapp.com/attachments/436071383836000256/467124893935337473/20180712_202157.jpg", // 25
            "https://cdn.discordapp.com/attachments/436071383836000256/467124892199026699/20180712_202313.jpg",
            "https://media.discordapp.net/attachments/436071383836000256/467124893935337472/20180712_202347.jpg",
            "https://media.discordapp.net/attachments/436071383836000256/467129242803765248/20180712_204441.jpg"
        };
    }


    public sealed class OctoNamePull : IService
    {
        public Task InitializeAsync()
            => Task.CompletedTask;

        public string[] OctoNameRu =
        {
            "[Фиолетовый дракон](https://i.imgur.com/jAnjdX0.jpg)",
            "[Красный дракон](https://i.imgur.com/liSAh1O.jpg)",
            "[Синий Дракон](https://i.imgur.com/XLQHSDC.jpg)",
            "[Серый дракон](https://i.imgur.com/Mrg8pXW.jpg)",


            "[Правильный Братишка](https://i.imgur.com/VbXuT55.jpg)",
            "[НеПравильный Братишка](https://i.imgur.com/VdIaiYT.jpg)",


            "[Белая Черешапка](https://i.imgur.com/c3TEfMM.jpg)",
            "[Жёлтая Пипашка](https://i.imgur.com/VAxfU84.jpg)",
            "[Розовенькая Черепашка](https://i.imgur.com/sEKy2Kv.jpg)",
            "[Голубая Черешапка](https://i.imgur.com/uPrFVVv.jpg)",
            "[Фиолетовая Черепашка](https://i.imgur.com/ULdmEfb.jpg)",


            "Жёлтый Буль", //
            "Красный Буль", //
            "[Зеленый Злюка](https://i.imgur.com/4LxTy7G.jpg)",
            "**????????**", // зеленый - другой
            "**???!!???**", // Розовый другой
            "[Радужный Злюка](https://i.imgur.com/CjUm1ix.jpg)",
            "[Небесный Осьминога](https://i.imgur.com/DCfBSPB.jpg)",
            "[Фиолетовый Буль](https://i.imgur.com/BijP5ne.jpg)",
            "[Пинки](https://i.imgur.com/d1DxlBV.jpg)",
            "[Зелёный Буль](https://i.imgur.com/L97rwTi.jpg)",
            "[Чёрно-Зелёный Зклюка](https://i.imgur.com/tyIrOO5.jpg)",
            "[Фиолетовый Няша](https://i.imgur.com/C23xBa5.jpg)",
            "[Краснющий Злюка](https://i.imgur.com/qmAMcZq.jpg)",
            "[Красненький Буль](https://i.imgur.com/PfdQDR3.jpg)",
            "[Красный буууу]()",
            "[Небесный Злюка](https://i.imgur.com/BCJnF28.jpg)",
            "[Голубой Буль](https://i.imgur.com/Uy1mM6g.jpg)",
            "[Чёрно-голубой буу](https://i.imgur.com/yWpDbCL.jpg)",
            "[Голубой Буль](https://i.imgur.com/Et0DX1b.jpg)",
            "[Синий Бу-Бу](https://i.imgur.com/lfIOzD1.jpg)",
            "[Голубой Бууу](https://i.imgur.com/PhNv8br.jpg)",
            "[Фиолетовый няша-близняша](https://i.imgur.com/7G68eev.jpg)",
            "[Пачи](https://i.imgur.com/T3sQ8Gz.jpg)",
            "[Злюка Пачи](https://i.imgur.com/BPmz5Jm.jpg)",
            "[Кууки](https://i.imgur.com/u6UQBNv.jpg)",
            "[Злюка Кууки](https://i.imgur.com/WaGqu52.jpg)",
            "[Серый Буль](https://i.imgur.com/VAhW7ij.jpg)",
            "[Чёрно-Серый Бууу](https://i.imgur.com/LHOfD3a.jpg)",
            "[Радужный](https://i.imgur.com/1x10cI1.jpg)"
            // "[]()"
        };
    }
}