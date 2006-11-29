package org.daisy.urakawa.media.asset;

import org.daisy.urakawa.media.MediaType;
import org.daisy.urakawa.xuk.XukAble;

/**
 *
 */
public interface MediaAsset extends XukAble {
    /**
     * @return convenience method that delegates to the AssetManager.
     * @see MediaAssetManager#getUidOfAsset(MediaAsset)
     */
    public String getUid();

    public String getName();

    public void setName(String name);

    /**
     *
     * @return
     */
    public MediaType getMediaType();

    /**
     *
     * @param man
     */
    public void setAssetManager(MediaAssetManager man);

    /**
     * @return can return NULL.
     */
    public MediaAssetManager getAssetManager();
}
