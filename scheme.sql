PGDMP      6                |            manager_bot     15.7 (Ubuntu 15.7-1.pgdg22.04+1)    16.0     "           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                      false            #           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                      false            $           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                      false            %           1262    16389    manager_bot    DATABASE     v   CREATE DATABASE manager_bot WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'en_US.utf8';
    DROP DATABASE manager_bot;
                grand    false            �            1259    16390    days    TABLE       CREATE TABLE public.days (
    date date NOT NULL,
    is_available boolean DEFAULT false NOT NULL,
    open_time time without time zone DEFAULT '08:00:00'::time without time zone NOT NULL,
    close_time time without time zone DEFAULT '20:00:00'::time without time zone NOT NULL
);
    DROP TABLE public.days;
       public         heap    grand    false            �            1259    16396    signs    TABLE       CREATE TABLE public.signs (
    username text NOT NULL,
    timespan integer NOT NULL,
    "time" time without time zone NOT NULL,
    date date NOT NULL,
    is_active boolean DEFAULT true NOT NULL,
    user_id bigint DEFAULT 0,
    id text DEFAULT '-'::text NOT NULL
);
    DROP TABLE public.signs;
       public         heap    grand    false            �            1259    16404    users    TABLE     �   CREATE TABLE public.users (
    user_id bigint NOT NULL,
    username text,
    created_at bigint NOT NULL,
    phone text,
    active boolean DEFAULT true NOT NULL
);
    DROP TABLE public.users;
       public         heap    grand    false                      0    16390    days 
   TABLE DATA           I   COPY public.days (date, is_available, open_time, close_time) FROM stdin;
    public          grand    false    214   �                 0    16396    signs 
   TABLE DATA           Y   COPY public.signs (username, timespan, "time", date, is_active, user_id, id) FROM stdin;
    public          grand    false    215   �                 0    16404    users 
   TABLE DATA           M   COPY public.users (user_id, username, created_at, phone, active) FROM stdin;
    public          grand    false    216   R#       �           2606    16410    users users_pkey 
   CONSTRAINT     S   ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (user_id);
 :   ALTER TABLE ONLY public.users DROP CONSTRAINT users_pkey;
       public            grand    false    216                 x���Kn�V@ѱ��]���d-g�?"�2������l���Qb���y���o|��o��Ϸ���c�<�<�=���������yy쭍�6��xk㭍�6��xk�-omyk�[[���֖�����-omyk���\�r�˅.�\�r�˅.�\�r�˅.�\�r�˅.�\�r�˅.�\�r�˅.�\�r�˅.���͍mnlsc������67���͍mnlsc������67���͍mnlsc������67���͍mnl��1�7�w���݅�.tw���]��Bw���݅�.tw���]��Bw���݅�.tw���]��Bw���݅�.tw�9��`N0'��	�s�9��`N0'��	�s�����W��+��z�J=|���RO_�����W��+���z<�ޟ�^���i����R�x�x���v��o��?������O�}���}���tߧ�>���O�}�{�{�{�{�{�{�{��:}p�>�N\��󫃋�_>�.�k�ϵ����s��v��˅^.�r���\��B/z��˅^.�r���\��B/z��˅^.�r���\��B/z��ۅ�.�v���]��Boz��ۅ�.�v���]��Boz��ۅ�.�v���]��Boz��ۅ�.�v�7�|�X�3�Z�1�򌱖g��<c��c-��kQ���[S���?�����_����x�<��<�c����[��3�������[���X��3�������[���5��[��2�c-1�c-1�c-1�c-1�c-1�c-1�c-1�c-1�c-1�c-1����y����=�?P�����:�|�WO�^~�3~R�?��ca����C���xXƀeXƀeXƀeX�Be,T�Be,T�Be,T�Be,T�Be,T�Be,T�Be,T�Be,T�Be,T�Be,T�Be,T�Be,T�Be,T�Be,T������-Yyf噕gV�Yyf噕gV�Yyf噕gV�Yyf噕gV�Yyf噕gV�Yyf噕gV�Yyf噕gV�Yyf噕gV�Yyf噕gV�Yyf噕gV�Yyf噕gV�Yyf噕gV�Yyf噕gV�Yyf噕gV�Yyfƙgf��qfƙgf��qfƙgf��qfƙgf��qfƙgf��qfƙgf��qfƙgf��qfƙgf��qfƙgf��qfƙgf��qfƙgf��qfƙgf��qfƙgf��qfƙgf��qfƙgf��qfƙgf��qfƙgf��qfƙgf��qfƙgf����co�	��������s�9���޲�O��;��=�>��s��.�p��=\��Bz��Å.�p��=\��Bz��Å.�p��=\��B%ȳ ς<�,ȳ ς<�,ȳ ς<�l����:[�l����:[�l����:[�l����:[�l����:[�l����:[�l����:[�l����:[�l����:[�l����:[�l����:[�l����:[�l����:[�l����:[�l����:[�l����:[�l����:[�l����:[�l����:[�l����:[�l����:[�l����:[�l����:[�l����:[�,��(΢8��,��(΢8��,��(΢8��,��(΢8��,��(΢8��,��(΢8��,��(΢8��,��(΢8��,��(΢8��,��(΢8���W3_�|5���W3_�|5���W3_�|5���W3_�|5���W3_�|5���W3_�|5���W3_�|5���W3_�|5���W3_�|5���o�oa�w��ǟ����e���ÿo�y�^�����������R            x���[�[9rǟ�����,ފ� �A� y��^�F�������������2��Z�"��!O�F���DNٜ��ՙ�s#�~�p���C�ѫ�'S�LEkv6��}���4����i?�l,�B�����6gI��qΊIR��ԙL��ju2f���ϥ��O���~��9}:��Đ�L'ad_}�>�V���K��Vn)i�)�p�n��d��ؘO]\����4����\j����4>>�7��x�R.ϟ�5���Z����:�����xo��as��Z���Ҳ?�Z������X�f,�����kU�jŇ�K��:Ŕ�)�X�k��Ue��h�w��M=��:mM繗_�����p����o�f�
���)eM9֑�Go�j�l�>)��ep�Y��9�|v��Z�Ƀ2��Jp��Ǐ���=<Z�\�ln��|����Yų�!�lɽ�)%z��1��wBVO3QÐ��/U"[�y������ݥ|zO���$�>�[�0u�;��Tb��r�Rm9�K�I��x~_?>�ӟ������5���һ��2|�(S���^
>f�ĩ����Y� �����bk�&O��G:��XX�!��ǔ��������ç���W+Z��Xw
!����d�7vh��Ӥ(�LW�<}z��\����/���[1f�Â�k1ɐ�����l�ե�b���P�]@��S�1E��FJ(�=��B��ݭ�0z�Yo�ZZH�[�]j(Cg��c󒤀�o���K��Tnw�*F�.zG-\�$q�Y�vl%��ԇȜ��a����^ S�3� dqY5�dCkΜ�>������Ͽ�w�}��Zof�%ʩ����{Y� ݵ�?6�}4itI��]�+�����G�3`9���S��|U��x;�b��Vl���,;��D���62�����X�/���>�kVq�ND<-*r��Ɣ�"��A�4���S�'�,ƈ�xr��J0�<f'�NEb�K�
�X�V,K��5�[N�R0�v�ޥ��#T��Z���'dɡ�	��0nyXM����J3��R��f���e����M��{��H��s�~Z�L9�oP�!`�Հ��A�ێ�}��0z�L���-�$��Ӑ8��e�����љ9�{����?����&L��)��-��Z�v�8��y3��{-k@D�˂�&�U〶�9�hO@3�P4I����0f1&�-3��{�\�<=�q[���bR��=�im��m��@%�f�f���z��ߎ�ģk�sbb����b%�4T���.�x1Z$K��.qNXeR�Ð�T-c�U�WJ�!������ 1�D����e�4����,����(<�|�:�;���d�@9��צ��W�+Sk\��}�Cp��)��C*���Zh��J&a DE��a�o7�*�����N������a^����nk��|yx|F��)��M�`��"A,� ���y�2��V[#z�TA�݀�-l��u�
�ځfmv�H�G9���}z�SZ2?W�Y�|h�#2���BP���I�!&�8[$��<쫛�ޅ��nN���ƥ?�;�E���c�K�ՌR/�a)�:�v�)XX���Z	""�auL�X�#��;��T����N)tS܄iC�Bz#ځ�[�Izһ�n͆�>��2��ℂbӄ!)�u5|i��8�� ��������3���c��l�_���t�o`��Mc+�!M���*��������!DB8Y\Ǵ�L�$��Pě�H#����z�W�������N�\0�Ou�v��˧�˛�^><\@�c2�KE%U�����|�JF�1!�^&_��q�^��f�
*���K��#p�b%�t�{�|�3,���0Zi��g����c*����ӿ<�/����k8
�TM�D�$��=֕�H�-�5���-��	g�Y	 9Z\�+3��W����{��)��Z��`:(	v�!��O4Kǫ�J�%L^��=��@�2�e���Lyy�Q���j3b!d��s�<�޾ŒH���s<>��p���Pڏ�gB�(�*[�v��ȼ�N��R].�À�hX�7i �׹D����v�K�5���G4�EO����Q�osC�������(�Sl�=�����B\�N�vA�W�c[J�$�j�A�h�te;��P~Ƨ
�Wt��0���z\�^���a�fIxݐ���!O��{y|���<�|��f8��� ���X���׃0
,;�κoX}��'{H��!{]e/���N\R㡽{�8��p�����bVQ�K���'5��PRSkh�#�����v<}|?>|�"�B��۰8u�=�C,C���Rl ���<�44ǿ~.�7����]�0>;����x����d�Ɩ��)�8�vrL���S��v}E����NSv���N7jj!�'��s��I�w
������ݨ8�e�;�cN�
�M�5s�%��=�L���1d�����	ɶ�\Σ���r���pS�Tk�:���{�KT���:7���	�~�~?�i�_Kd� T�Y��Hm�;����p��DG�1�U��Y+��Vd,�l�j������א�V���3M��c��$�ؕ�?��'��� 1E�eܶٚG���9���Z �[z_Z��d�2΢"��H�G���)O��I��.p��t�)�����F�AWP]W�W���=���z� ���{u�kJ�,j����Auve�(�����q�`^�6�"��]�P���aovB��]q+�r�"EEk\'R���0�6 (������q����])sœt2�e���Q\M��:�̢����[�m�[d5�p^>����P��q����j��5��C�:T�ل�� |�m�c�`OfROA&~�U�#)
���I����ϯ�~����g
;�}4E^W�/1�cCf����:`Xh<���Ѭ+@���1�;g��z�;2��5�,��4#�5�yH�R�;��uP���1�SgWZ)c�R��˾ټ�v�qMMH|ܥ1���*���M6�on���k�Q��ψs���Y6�[~e�,̥������K��k
��gE��%�����v��q�g��.X[�u�4|� ��MW&#-_O.$������KE�2%ԛ��b+d]Bx�i��M	kl�S��o"c��	n6�4��At2�L�Y=2m���(�p["��L� �iڷu�DC6�NY]D���q�����e���~���S����#V�^X�� [\�����l�M�~���R�NZm�����l�X��[����]E��֦t����uĒ�.%��1�u���ʇ�s��RXӈ
���T A맲�q�~G�y���%V�N�ގuY�f�'N�Au�s�C���5�3|� ��22�-���Bk;�s������b��!c���u�C��R�2~z�u=��q6S��ӯ�8��b��a�܎�hp(�n� �ou���(���1{b�ܒ�w�d�t�X�߈,/1�����0>�K�ݏ����UD������A�K�!_/���$n���K���	 k&��^*(o�������*�*`���8\3�z<4t[d��-s��y�c|Q+� ��d+*x�����z�]7�֌4yo�H�kq���a:޽���k��/���A��h��u4E��E�Z;ptv�^�^'� �X���aޭ���j���H"��W7+��]/ݬA�����\tH&���j;��SO$��c��`;%��s�lW��hj��j�uQpꈩn!����h<kd�Qb��	=^Kxz���|���ר�����'�i$m�>
�[����:�6���	��ق>&�������jɎ:�FK�yދ�6р~����"�ZJ����5�Ϲ�n�����ė��z�4��'��������Q+VZ�[Ƌ�f�1L$z�E��=ɏx��6@��(@4���h���^�w̺���1���K�m� �N�.�� S��?�Q_w.N[��֓U��K�dVo���S�WW���(� � }SA�M\����?�n;ĺF�_�v�0��̸%��^�mn`�>A�� �   ��l�h�YZ�M.57|b��0�!ޡ�&� ��G("�j�9\��n�w�����]�
"Xm���JK:`_hs��3��a�U�/XO��Y��|Y\b��)dp��j�SހB�v�1�T���|>�����         
  x�UU]��6|�����.�����{i� /�قe� ��_ߡD�R��8܏ٙ��z�A��a7�q۝��E��{�����9?��,�<��t����[�V��ļ"q�XN���>_�\a�z���XT�n�^ƾ�mq�K�1�`>l^�������\���'K��7/��_N^*x_C���3��ۜ��y�w��
�O+�\4_�<� �����<w������Kx�3c��7��+.�V���yK���t��пt+�Y���"ʁ�qڏ��wC~�.�ڈ�I4�R�������J�U&�)xG�O�co5q�\�¬�Ѵ��ۤ�Հ	��8<^��mh�$���H��/�Ϸ���-�8n=�*4�O�ي��*��X<�H'��k��t�)�sv�@ҽb�Z��Vc��$]b�$�Ĥ�"�$�W$����c̻.�[���Q[F��|�Ӹy����V\�&:�_��l�|{����
���
''.%ṡS�l��i���<Rw�X�j��P={ԏ�BQ�J��#��$�� C�I"<XU�0Nl����]�r%��;kѧ�g�b@pi%��S4V�9����R,��g���[$#��!1���l���Q��\Yy��R,��K�r
^���� ,GHk��;�y����i�4�H�spZ敬y�C�y��"�[%	d0��p��w���F\�Q0�&n=�C��B�V�ulz�#{���a$e����P5��EX�!�.����Ē�a;�ڕY{�������ؠ8�k)�z9$O�G?vX���<��i��0-�}y`�P|�Z�@ѭ�{�o!��o��;�$`�p�8�XM�����5�`�����/����2�Zv�n�,����?`ᒆ��#>��V�2}iu	/0`�h��?]�ˆ��L�:4�p�s���t=�-���H�rL*o�{ZvRZ"jd�-� Gx	�~���6w�m!���zE@I�*ȃl�{#P)�҈/u����E)�<�2�*�o�><<�]��u     